using System;
using System.Collections.Generic;

using Microsoft.DirectX.DirectInput;
using Microsoft.Xna.Framework.Input;

namespace Soopah.Xna.Input
{
    public class DirectInputGamepad
    {
		protected static List<DirectInputGamepad> gamepads;
		public static List<DirectInputGamepad> Gamepads
		{
			get
			{
                if (gamepads == null)
                {
                    // gamepads generally misidentified as Joysticks in DirectInput... get both
                    DeviceList gamepadInstanceList = Manager.GetDevices(DeviceType.Gamepad, EnumDevicesFlags.AttachedOnly);
                    DeviceList joystickInstanceList = Manager.GetDevices(DeviceType.Joystick, EnumDevicesFlags.AttachedOnly);

                    //ReloadGamepads();
                }

				return gamepads;
			}
		}

		/// <summary>
		/// Normally for internal use only; call if user has attached new Gamepads,
		/// or detached Gamepads you want discarded
		/// Otherwise, loaded once on first Gamepad request and does not reflect changes in gamepad attachment
		/// TODO: Do this better
		/// </summary>
		public static void ReloadGamepads()
		{
			// gamepads generally misidentified as Joysticks in DirectInput... get both
			DeviceList gamepadInstanceList = Manager.GetDevices(DeviceType.Gamepad, EnumDevicesFlags.AttachedOnly);
			DeviceList joystickInstanceList = Manager.GetDevices(DeviceType.Joystick, EnumDevicesFlags.AttachedOnly);

			gamepads = new List<DirectInputGamepad>(gamepadInstanceList.Count + joystickInstanceList.Count);

			foreach (DeviceInstance deviceInstance in gamepadInstanceList)
			{
				DirectInputGamepad gamepad = new DirectInputGamepad(deviceInstance.InstanceGuid);
				gamepads.Add(gamepad);
			}
			foreach (DeviceInstance deviceInstance in joystickInstanceList)
			{
				DirectInputGamepad gamepad = new DirectInputGamepad(deviceInstance.InstanceGuid);
				gamepads.Add(gamepad);
			}
		}


		protected Device device;
		public Device Device
		{
			get { return device; }
		}

		protected DirectInputGamepad(Guid gamepadInstanceGuid)
		{
			device = new Device(gamepadInstanceGuid);
			device.SetDataFormat(DeviceDataFormat.Joystick);
			device.Acquire();			
		}

		public DirectInputThumbSticks ThumbSticks
		{
			get { return new DirectInputThumbSticks(device); }
		}

		public DirectInputDPad DPad
		{
			get
			{
				JoystickState t = device.CurrentJoystickState;
				return new DirectInputDPad(t.GetPointOfView()[0]);	// note that there could be a total of 4 DPads on the PC
			}
		}

		public DirectInputButtons Buttons
		{
			get { return new DirectInputButtons(device); }
		}

		#region Diagnostics
		public string DiagnosticsThumbSticks
		{
			get
			{
				return
					"X" + Math.Round(ThumbSticks.Left.X, 4) +
					" Y" + Math.Round(ThumbSticks.Left.Y, 4) +
					" X" + Math.Round(ThumbSticks.Right.X, 4) +
					" Y" + Math.Round(ThumbSticks.Right.Y, 4);
			}
		}

		public string DiagnosticsRawGamepadData
		{
			get
			{
				return
					"X" + Device.CurrentJoystickState.X +
					" Y" + Device.CurrentJoystickState.Y +
					" Z" + Device.CurrentJoystickState.Z +
					" Rx" + Device.CurrentJoystickState.Rx +
					" Ry" + Device.CurrentJoystickState.Ry +
					" Rz" + Device.CurrentJoystickState.Rz +
					" pov[0]" + Device.CurrentJoystickState.GetPointOfView()[0];
			}
		}

		public string DiagnosticsButtons
		{
			get
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder();

				int i = 0;
				foreach (ButtonState bs in Buttons.List)
				{
					sb.Append(i);
					sb.Append("=");
					sb.Append((bs == ButtonState.Pressed ? "1" : "0"));
					sb.Append(" ");
					i++;
				}

				return sb.ToString();
			}
		}
		#endregion
	}
}
