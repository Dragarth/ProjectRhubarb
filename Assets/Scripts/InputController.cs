using UnityEngine;
using System.Collections;
using XInputDotNetPure;

public class InputController : MonoBehaviour
{
    bool playerIndexSet = false;

    PlayerIndex playerIndex;
    GamePadState state;
    GamePadState prevState;



    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!playerIndexSet)
            return;

        prevState = state;
        state = GamePad.GetState(playerIndex);
    }

    public void SetPlayer(int playerNumber)
    {
        if (playerNumber > 4)
        {
            Debug.Log("Player Number out of bounds: " + playerNumber);
            return;
        }
        playerIndex = (PlayerIndex)(playerNumber - 1);
        playerIndexSet = true;
    }

    #region Button Inputs

    public bool PressedA()
    {
        bool pressed = false;
        if (prevState.Buttons.A == ButtonState.Released && state.Buttons.A == ButtonState.Pressed)
            pressed = true;
        return pressed;
    }

    public bool PressedB()
    {
        bool pressed = false;
        if (prevState.Buttons.B == ButtonState.Released && state.Buttons.B == ButtonState.Pressed)
            pressed = true;
        return pressed;
    }

    public bool PressedX()
    {
        bool pressed = false;
        if (prevState.Buttons.X == ButtonState.Released && state.Buttons.X == ButtonState.Pressed)
            pressed = true;
        return pressed;
    }

    public bool PressedY()
    {
        bool pressed = false;
        if (prevState.Buttons.Y == ButtonState.Released && state.Buttons.Y == ButtonState.Pressed)
            pressed = true;
        return pressed;
    }

    public bool PressedBack()
    {
        bool pressed = false;
        if (prevState.Buttons.Back == ButtonState.Released && state.Buttons.Back == ButtonState.Pressed)
            pressed = true;
        return pressed;
    }

    public bool PressedStart()
    {
        bool pressed = false;
        if (prevState.Buttons.Start == ButtonState.Released && state.Buttons.Start == ButtonState.Pressed)
            pressed = true;
        return pressed;
    }

    public bool PressedLeftShoulder()
    {
        bool pressed = false;
        if (prevState.Buttons.LeftShoulder == ButtonState.Released && state.Buttons.LeftShoulder == ButtonState.Pressed)
            pressed = true;
        return pressed;
    }

    public bool PressedRightShoulder()
    {
        bool pressed = false;
        if (prevState.Buttons.RightShoulder == ButtonState.Released && state.Buttons.RightShoulder == ButtonState.Pressed)
            pressed = true;
        return pressed;
    }

    public bool PressedGuide()
    {
        bool pressed = false;
        if (prevState.Buttons.Guide == ButtonState.Released && state.Buttons.Guide == ButtonState.Pressed)
            pressed = true;
        return pressed;
    }

    #endregion

    #region DPad Inputs

    public bool PressedDUp()
    {
        bool pressed = false;
        if (prevState.DPad.Up == ButtonState.Released && state.DPad.Up == ButtonState.Pressed)
            pressed = true;
        return pressed;
    }

    public bool PressedDDown()
    {
        bool pressed = false;
        if (prevState.DPad.Down == ButtonState.Released && state.DPad.Down == ButtonState.Pressed)
            pressed = true;
        return pressed;
    }

    public bool PressedDLeft()
    {
        bool pressed = false;
        if (prevState.DPad.Left == ButtonState.Released && state.DPad.Left == ButtonState.Pressed)
            pressed = true;
        return pressed;
    }

    public bool PressedDRight()
    {
        bool pressed = false;
        if (prevState.DPad.Right == ButtonState.Released && state.DPad.Right == ButtonState.Pressed)
            pressed = true;
        return pressed;
    }
    #endregion

    #region Analogue Inputs

    public float LeftHorizontal()
    {
        return state.ThumbSticks.Left.X;
    }

    public float LeftVertical()
    {
        return state.ThumbSticks.Left.Y;
    }

    public float RightHorizontal()
    {
        return state.ThumbSticks.Right.X;
    }

    public float RightVertical()
    {
        return state.ThumbSticks.Right.Y;
    }

    public float LeftTrigger()
    {
        return state.Triggers.Left;
    }

    public float RightTrigger()
    {
        return state.Triggers.Right;
    }

    #endregion
}
