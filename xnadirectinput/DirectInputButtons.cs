using System;
using System.Collections.Generic;
using Microsoft.DirectX.DirectInput;
using Microsoft.Xna.Framework.Input;

namespace Soopah.Xna.Input
{
	/// <summary>
	/// The buttons on a PC Gamepad
	/// </summary>
	/// <remarks>
	/// The comparable X, Y, A, B etc are not currently mapped because the position of buttons
	/// indexed 0, 1, 2, etc varies widely from PC gamepad to PC gamepad
	/// Instead, you should provide an interface for the user to visually map the buttons on their
	/// gamepad to what you expect for X, Y, etc.
	/// Tools to support this mapping may be added to this framework in the future
	/// </remarks>
	public struct DirectInputButtons
	{
		/*
		public ButtonState X;
		public ButtonState Y;
		public ButtonState A;
		public ButtonState B;
		public ButtonState Back;
		public ButtonState Start;
		public ButtonState LeftShoulder;
		public ButtonState RightShoulder;
		public ButtonState LeftStick;
		public ButtonState RightStick;
		*/

		public List<ButtonState> List;

		public DirectInputButtons(Device device)
		{
			byte[] buttons = device.CurrentJoystickState.GetButtons();

			/*
			X = (buttons[0] == 0 ? ButtonState.Released : ButtonState.Pressed);
			Y = (buttons[1] == 0 ? ButtonState.Released : ButtonState.Pressed);
			A = (buttons[2] == 0 ? ButtonState.Released : ButtonState.Pressed);
			B = (buttons[3] == 0 ? ButtonState.Released : ButtonState.Pressed);
			Back = (buttons[4] == 0 ? ButtonState.Released : ButtonState.Pressed);
			Start = (buttons[5] == 0 ? ButtonState.Released : ButtonState.Pressed);
			LeftShoulder = (buttons[6] == 0 ? ButtonState.Released : ButtonState.Pressed);
			RightShoulder = (buttons[7] == 0 ? ButtonState.Released : ButtonState.Pressed);
			LeftStick = (buttons[8] == 0 ? ButtonState.Released : ButtonState.Pressed);
			RightStick = (buttons[9] == 0 ? ButtonState.Released : ButtonState.Pressed);
			*/

			int numButtons = device.Caps.NumberButtons;
			List = new List<ButtonState>(numButtons);

			for (int i = 0; i < numButtons; i++)
			{
				List.Add((buttons[i] == 0 ? ButtonState.Released : ButtonState.Pressed));
			}
		}
	}
}
