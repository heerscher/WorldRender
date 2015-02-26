
namespace WorldRender.Input
{
    public interface IBinding
    {
        bool IsPressed(IState inputState);
        bool IsDown(IState inputState);
    }
}
