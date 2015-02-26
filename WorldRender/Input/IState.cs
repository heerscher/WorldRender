
namespace WorldRender.Input
{
    public interface IState
    {
        bool IsKeyDown(System.Windows.Forms.Keys key);
        bool IsKeyPressed(System.Windows.Forms.Keys key);

        bool IsMouseButtonDown(System.Windows.Forms.MouseButtons mouseButton);
        bool IsMouseButtonPressed(System.Windows.Forms.MouseButtons mouseButton);

        int MouseX();
        int MouseY();

        int MouseDeltaX();
        int MouseDeltaY();

        void UpdateState();

        Command Register(string name);
    }
}
