
namespace WorldRender.Input
{
    public interface IState
    {
        bool IsKeyDown(System.Windows.Forms.Keys key);
        bool IsKeyPressed(System.Windows.Forms.Keys key);

        int MouseX();
        int MouseY();

        int MouseDeltaX();
        int MouseDeltaY();

        void UpdateState();
    }
}
