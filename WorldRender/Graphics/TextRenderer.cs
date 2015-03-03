using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldRender.Graphics
{
    internal class TextRenderer : IDisposable
    {
        private SlimDX.Direct3D10_1.Device1 device;
        private SlimDX.DXGI.KeyedMutex mutex10;
        private SlimDX.DXGI.KeyedMutex mutex11;
        private SlimDX.Direct2D.RenderTarget dwRenderTarget;
        private SlimDX.DirectWrite.TextFormat textFormat;
        private SlimDX.Direct3D11.BlendState BlendState_Transparent;
        private SlimDX.Direct3D11.Texture2D textureD3D11;
        private SlimDX.Direct2D.SolidColorBrush brushSolidWhite;
        private SlimDX.Direct3D11.Effect effect;
        private SlimDX.Direct3D11.InputLayout layoutText;
        private SlimDX.Direct3D11.Buffer vertexBufferText;

        public TextRenderer(Device device)
        {
            using (var factory = new SlimDX.DXGI.Factory1())
            {
                using (var adapter = factory.GetAdapter1(0))
                {
                    this.device = new SlimDX.Direct3D10_1.Device1(
                        adapter,
                        SlimDX.Direct3D10.DriverType.Hardware,
                        SlimDX.Direct3D10.DeviceCreationFlags.BgraSupport,
                        SlimDX.Direct3D10_1.FeatureLevel.Level_10_0
                    );

                    // Create the DirectX11 texture2D. This texture will be shared with the DirectX10
                    // device. The DirectX10 device will be used to render text onto this texture. DirectX11
                    // will then draw this texture (blended) onto the screen.
                    // The KeyedMutex flag is required in order to share this resource.
                    textureD3D11 = new SlimDX.Direct3D11.Texture2D(device.Handle, new SlimDX.Direct3D11.Texture2DDescription
                    {
                        Width = device.Form.Width,
                        Height = device.Form.Height,
                        MipLevels = 1,
                        ArraySize = 1,
                        Format = SlimDX.DXGI.Format.B8G8R8A8_UNorm,
                        SampleDescription = new SlimDX.DXGI.SampleDescription(1, 0),
                        Usage = SlimDX.Direct3D11.ResourceUsage.Default,
                        BindFlags = SlimDX.Direct3D11.BindFlags.RenderTarget | SlimDX.Direct3D11.BindFlags.ShaderResource,
                        CpuAccessFlags = SlimDX.Direct3D11.CpuAccessFlags.None,
                        OptionFlags = SlimDX.Direct3D11.ResourceOptionFlags.KeyedMutex
                    });

                    // A DirectX10 Texture2D sharing the DirectX11 Texture2D
                    var sharedResource = new SlimDX.DXGI.Resource(textureD3D11);
                    var textureD3D10 = this.device.OpenSharedResource<SlimDX.Direct3D10.Texture2D>(sharedResource.SharedHandle);

                    // The KeyedMutex is used just prior to writing to textureD3D11 or textureD3D10.
                    // This is how DirectX knows which DirectX (10 or 11) is supposed to be writing
                    // to the shared texture.  The keyedMutex is just defined here, they will be used
                    // a bit later.
                    mutex10 = new SlimDX.DXGI.KeyedMutex(textureD3D10);
                    mutex11 = new SlimDX.DXGI.KeyedMutex(textureD3D11);

                    // Direct2D Factory
                    SlimDX.Direct2D.Factory d2Factory = new SlimDX.Direct2D.Factory(
                        SlimDX.Direct2D.FactoryType.SingleThreaded,
                        SlimDX.Direct2D.DebugLevel.Information
                    );

                    // Direct Write factory
                    SlimDX.DirectWrite.Factory dwFactory = new SlimDX.DirectWrite.Factory(
                        SlimDX.DirectWrite.FactoryType.Isolated
                    );

                    // The textFormat we will use to draw text with
                    textFormat = new SlimDX.DirectWrite.TextFormat(
                        dwFactory,
                        "Arial",
                        SlimDX.DirectWrite.FontWeight.Normal,
                        SlimDX.DirectWrite.FontStyle.Normal,
                        SlimDX.DirectWrite.FontStretch.Normal,
                        24,
                        "en-US"
                    );
                    textFormat.TextAlignment = SlimDX.DirectWrite.TextAlignment.Center;
                    textFormat.ParagraphAlignment = SlimDX.DirectWrite.ParagraphAlignment.Center;

                    // Query for a IDXGISurface.
                    // DirectWrite and DirectX10 can interoperate thru DXGI.
                    var surface = textureD3D10.AsSurface();
                    var rtp = new SlimDX.Direct2D.RenderTargetProperties();
                    rtp.MinimumFeatureLevel = SlimDX.Direct2D.FeatureLevel.Direct3D10;
                    rtp.Type = SlimDX.Direct2D.RenderTargetType.Hardware;
                    rtp.Usage = SlimDX.Direct2D.RenderTargetUsage.None;
                    rtp.PixelFormat = new SlimDX.Direct2D.PixelFormat(SlimDX.DXGI.Format.Unknown, SlimDX.Direct2D.AlphaMode.Premultiplied);
                    dwRenderTarget = SlimDX.Direct2D.RenderTarget.FromDXGI(d2Factory, surface, rtp);
                    
                    // Brush used to DrawText
                    brushSolidWhite = new SlimDX.Direct2D.SolidColorBrush(
                        dwRenderTarget,
                        new SlimDX.Color4(1, 1, 1, 1)
                    );

                    // Think of the shared textureD3D10 as an overlay.
                    // The overlay needs to show the text but let the underlying triangle (or whatever)
                    // show thru, which is accomplished by blending.
                    var bsd = new SlimDX.Direct3D11.BlendStateDescription();
                    bsd.RenderTargets[0].BlendEnable = true;
                    bsd.RenderTargets[0].SourceBlend = SlimDX.Direct3D11.BlendOption.SourceAlpha;
                    bsd.RenderTargets[0].DestinationBlend = SlimDX.Direct3D11.BlendOption.InverseSourceAlpha;
                    bsd.RenderTargets[0].BlendOperation = SlimDX.Direct3D11.BlendOperation.Add;
                    bsd.RenderTargets[0].SourceBlendAlpha = SlimDX.Direct3D11.BlendOption.One;
                    bsd.RenderTargets[0].DestinationBlendAlpha = SlimDX.Direct3D11.BlendOption.Zero;
                    bsd.RenderTargets[0].BlendOperationAlpha = SlimDX.Direct3D11.BlendOperation.Add;
                    bsd.RenderTargets[0].RenderTargetWriteMask = SlimDX.Direct3D11.ColorWriteMaskFlags.All;
                    BlendState_Transparent = SlimDX.Direct3D11.BlendState.FromDescription(device.Handle, bsd);

                    // Load Effect. This includes both the vertex and pixel shaders.
                    // Also can include more than one technique.
                    var shaderByteCode = SlimDX.D3DCompiler.ShaderBytecode.CompileFromFile(
                        "texteffect.fx",
                        "fx_5_0",
                        SlimDX.D3DCompiler.ShaderFlags.EnableStrictness,
                        SlimDX.D3DCompiler.EffectFlags.None);

                    effect = new SlimDX.Direct3D11.Effect(device.Handle, shaderByteCode);

                    // create triangle vertex data, making sure to rewind the stream afterward
                    var verticesTriangle = new SlimDX.DataStream(30 * 3, true, true);
                    verticesTriangle.Write(new SlimDX.Vector3(0.0f, 0.5f, 0.5f));
                    verticesTriangle.Write(new SlimDX.Color4(1.0f, 0.0f, 0.0f, 1.0f));
                    verticesTriangle.Write(new SlimDX.Vector3(0.5f, -0.5f, 0.5f));
                    verticesTriangle.Write(new SlimDX.Color4(0.0f, 1.0f, 0.0f, 1.0f));
                    verticesTriangle.Write(new SlimDX.Vector3(-0.5f, -0.5f, 0.5f));
                    verticesTriangle.Write(new SlimDX.Color4(0.0f, 0.0f, 1.0f, 1.0f));
                    verticesTriangle.Position = 0;

                    // create the triangle vertex layout and buffer
                    var inputElements = new SlimDX.Direct3D11.InputElement[] {
                new SlimDX.Direct3D11.InputElement("POSITION", 0, SlimDX.DXGI.Format.R32G32B32A32_Float, 0, 0),
                new SlimDX.Direct3D11.InputElement("COLOR",0,SlimDX.DXGI.Format.R32G32B32A32_Float,16,0)
            };
                    var layoutColor = new SlimDX.Direct3D11.InputLayout(device.Handle, effect.GetTechniqueByName("Color").GetPassByIndex(0).Description.Signature, inputElements);
                    var vertexBufferColor = new SlimDX.Direct3D11.Buffer(device.Handle, verticesTriangle, (int)verticesTriangle.Length, SlimDX.Direct3D11.ResourceUsage.Default, SlimDX.Direct3D11.BindFlags.VertexBuffer, SlimDX.Direct3D11.CpuAccessFlags.None, SlimDX.Direct3D11.ResourceOptionFlags.None, 0);
                    verticesTriangle.Close();

                    // create text vertex data, making sure to rewind the stream afterward
                    // Top Left of screen is -1, +1
                    // Bottom Right of screen is +1, -1
                    var verticesText = new SlimDX.DataStream(30 * 4, true, true);
                    verticesText.Write(new SlimDX.Vector3(-1, 1, 0));
                    verticesText.Write(new SlimDX.Vector2(0, 0f));
                    verticesText.Write(new SlimDX.Vector3(1, 1, 0));
                    verticesText.Write(new SlimDX.Vector2(1, 0));
                    verticesText.Write(new SlimDX.Vector3(-1, -1, 0));
                    verticesText.Write(new SlimDX.Vector2(0, 1));
                    verticesText.Write(new SlimDX.Vector3(1, -1, 0));
                    verticesText.Write(new SlimDX.Vector2(1, 1));
                    verticesText.Position = 0;

                    // create the text vertex layout and buffer
                    layoutText = new SlimDX.Direct3D11.InputLayout(device.Handle, effect.GetTechniqueByName("Text").GetPassByIndex(0).Description.Signature, inputElements);
                    var vertexBufferText = new SlimDX.Direct3D11.Buffer(device.Handle, verticesText, (int)verticesText.Length, SlimDX.Direct3D11.ResourceUsage.Default, SlimDX.Direct3D11.BindFlags.VertexBuffer, SlimDX.Direct3D11.CpuAccessFlags.None, SlimDX.Direct3D11.ResourceOptionFlags.None, 0);
                    verticesText.Close();

                }
            }
        }

        public void Render(Device device)
        {
                            // Draw Text on the shared Texture2D
                // Need to Acquire the shared texture for use with DirectX10
                mutex10.Acquire(0, 100);
                dwRenderTarget.BeginDraw();
                dwRenderTarget.Clear(new SlimDX.Color4(0, 0, 0, 0));
                const string text = "Hello world!";
                dwRenderTarget.DrawText(text, textFormat, new System.Drawing.Rectangle(0, 0, device.Form.Width, device.Form.Height), brushSolidWhite);
                dwRenderTarget.EndDraw();
                mutex10.Release(0);
 
                // Draw the shared texture2D onto the screen
                // Need to Aquire the shared texture for use with DirectX11
                mutex11.Acquire(0, 100);
                var srv = new SlimDX.Direct3D11.ShaderResourceView(device.Handle, textureD3D11);
                effect.GetVariableByName("g_textOverlay").AsResource().SetResource(srv);
                device.Context.InputAssembler.InputLayout = layoutText;
                device.Context.InputAssembler.PrimitiveTopology = SlimDX.Direct3D11.PrimitiveTopology.TriangleStrip;
                device.Context.InputAssembler.SetVertexBuffers(0, new SlimDX.Direct3D11.VertexBufferBinding(vertexBufferText, 30 * 4, 0));
                device.Context.OutputMerger.BlendState = BlendState_Transparent;
                var currentTechnique = effect.GetTechniqueByName("Text");
                for (int pass = 0; pass < currentTechnique.Description.PassCount; ++pass)
                {
                    var Pass = currentTechnique.GetPassByIndex(pass);
                    System.Diagnostics.Debug.Assert(Pass.IsValid, "Invalid EffectPass");
                    Pass.Apply(device.Context);
                    device.Context.Draw(4, 0);
                }
                srv.Dispose();
                mutex11.Release(0);
 
        }

        public void Dispose()
        {
            if (device != null)
            {
                device.Dispose();
                device = null;
            }
        }
    }
}
