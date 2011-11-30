using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Soopah.Xna.Input
{
	public struct DirectInputDPad
	{
		public ButtonState Up;
		public ButtonState Right;
		public ButtonState Down;
		public ButtonState Left;

		public DirectInputDPad(int direction)
		{
			Up = ButtonState.Released;
			Right = ButtonState.Released;
			Down = ButtonState.Released;
			Left = ButtonState.Released;

			if (direction == -1)
				return;

			if (direction > 27000 || direction < 9000)
				Up = ButtonState.Pressed;

			if (0 < direction && direction < 18000)
				Right = ButtonState.Pressed;

			if (9000 < direction && direction < 27000)
				Down = ButtonState.Pressed;

			if (18000 < direction)
				Left = ButtonState.Pressed;
		}
	}
}
