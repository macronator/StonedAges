using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Engine;

public class Keyboard
{
    private class KeyState
    {
        private bool _keyPressDetected;

        public bool Held { get; set; }

        public bool Pressed { get; set; }

        public KeyState()
        {
            Held = false;
            Pressed = false;
        }

        internal void OnDown()
        {
            if (!Held)
            {
                _keyPressDetected = true;
            }
            Held = true;
        }

        internal void OnUp()
        {
            Held = false;
        }

        internal void Process()
        {
            Pressed = false;
            if (_keyPressDetected)
            {
                rate = 300;
                _repeatRate = DateTime.UtcNow;
                Pressed = true;
                _keyPressDetected = false;
            }
        }
    }

    private static DateTime _repeatRate = DateTime.UtcNow;

    private static int rate = 300;

    private Control _openGLControl;

    public KeyPressEventHandler KeyPressEvent;

    private Dictionary<Keys, KeyState> _keyStates = new Dictionary<Keys, KeyState>();

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("User32.dll")]
    public static extern short GetAsyncKeyState(int vKey);

    public Keyboard(Control openGLControl)
    {
        _openGLControl = openGLControl;
        _openGLControl.KeyDown += OnKeyDown;
        _openGLControl.KeyUp += OnKeyUp;
        _openGLControl.KeyPress += OnKeyPress;
    }

    private void OnKeyPress(object sender, KeyPressEventArgs e)
    {
        if (KeyPressEvent != null)
        {
            KeyPressEvent(sender, e);
        }
    }

    private void OnKeyUp(object sender, KeyEventArgs e)
    {
        EnsureKeyStateExists(e.KeyCode);
        _keyStates[e.KeyCode].OnUp();
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        EnsureKeyStateExists(e.KeyCode);
        _keyStates[e.KeyCode].OnDown();
    }

    private void EnsureKeyStateExists(Keys key)
    {
        if (!_keyStates.Keys.Contains(key))
        {
            _keyStates.Add(key, new KeyState());
        }
    }

    public bool IsKeyPressedOrHeld(Keys key)
    {
        EnsureKeyStateExists(key);
        if (_keyStates[key].Held)
        {
            return _keyStates[key].Held;
        }
        return _keyStates[key].Pressed;
    }

    public bool IsKeyPressed(Keys key)
    {
        EnsureKeyStateExists(key);
        return _keyStates[key].Pressed;
    }

    public bool IsKeyPressedAndRepeated(Keys key)
    {
        EnsureKeyStateExists(key);
        if (_keyStates[key].Held && DateTime.UtcNow.Subtract(_repeatRate).TotalMilliseconds > (double)rate)
        {
            rate = 50;
            _repeatRate = DateTime.UtcNow;
            _keyStates[key].Pressed = true;
            return _keyStates[key].Pressed;
        }
        return _keyStates[key].Pressed;
    }

    public bool IsKeyHeld(Keys key)
    {
        EnsureKeyStateExists(key);
        return _keyStates[key].Held;
    }

    public void Process()
    {
        ProcessControlKeys();
        foreach (KeyState value in _keyStates.Values)
        {
            value.Pressed = false;
            value.Process();
        }
    }

    private bool PollKeyPress(Keys key)
    {
        return GetAsyncKeyState((int)key) != 0;
    }

    private void ProcessControlKeys()
    {
        UpdateControlKey(Keys.Tab);
        UpdateControlKey(Keys.Left);
        UpdateControlKey(Keys.Right);
        UpdateControlKey(Keys.Up);
        UpdateControlKey(Keys.Down);
        UpdateControlKey(Keys.LMenu);
    }

    private void UpdateControlKey(Keys keys)
    {
        if (PollKeyPress(keys))
        {
            OnKeyDown(this, new KeyEventArgs(keys));
        }
        else
        {
            OnKeyUp(this, new KeyEventArgs(keys));
        }
    }
}
