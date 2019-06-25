using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyInput : Singleton<MyInput> {

    [SerializeField] List<MyInputButton> buttons;
    [SerializeField] List<MyInputAxis> axes;
    [SerializeField] List<MyInputAxisAsButton> axis_buttons;

    Dictionary<string, IMyInputButton> button_dict;
    Dictionary<string, MyInputAxis> axis_dict;

    public static bool GetButtonDown(string name) {
        return instance ? instance.button_dict[name].IsDown() : Input.GetButtonDown(name);
    }

    public static bool GetButtonDown(string name, float buffer) {
        return instance ? instance.button_dict[name].IsDownBuffered(buffer) : Input.GetButtonDown(name);
    }

    public static bool GetButton(string name) {
        return instance ? instance.button_dict[name].is_held : Input.GetButton(name);
    }

    public static float GetAxis(string name) {
        return instance ? instance.axis_dict[name].value : Input.GetAxisRaw(name);
    }

    public static bool ClearBuffer(string name) {
        if (instance == null) {
            return false;
        }
        if (instance.button_dict.ContainsKey(name)) {
            instance.button_dict[name].ClearBuffer();
            return true;
        }
        return false;
    }

    protected override void OnAwake() {
        button_dict = new Dictionary<string, IMyInputButton>();

        axis_dict = new Dictionary<string, MyInputAxis>();

        foreach (MyInputButton button in buttons) {
            button_dict.Add(button.name, button);
        }
        foreach (MyInputAxisAsButton button in axis_buttons) {
            button_dict.Add(button.name, button);
        }
        foreach (MyInputAxis axis in axes) {
            axis_dict.Add(axis.name, axis);
        }
    }

    private void Update() {
        foreach (MyInputButton button in buttons) {
            button.Update(!GameManager.instance.input_active);
        }
        foreach (MyInputAxisAsButton button in axis_buttons) {
            button.Update(!GameManager.instance.input_active);
        }
    }

    interface IMyInputButton {
        bool is_held { get; }

        bool IsDown();
        bool IsDownBuffered(float buffer_length);
        void Update(bool is_paused);
        void ClearBuffer();
    }

    [Serializable]
    class MyInputButton : IMyInputButton {
        public bool is_held { get; private set; }

        public string name;

        bool is_down;

        float last_down = float.MinValue;


        public void Update(bool is_paused) {
            is_down = false;
            if (Input.GetButton(name)) {
                if (!is_held) {
                    if (!is_paused) last_down = Time.unscaledTime;
                    is_down = true;
                }
                is_held = true;
            } else {
                is_held = false;
            }
        }

        public bool IsDown() {
            return is_down;
        }
        public bool IsDownBuffered(float buffer_length) {
            bool to_ret = last_down + buffer_length >= Time.unscaledTime;
            return to_ret;
        }

        public void ClearBuffer() {
            last_down = 0f;
        }
    }

    [Serializable]
    class MyInputAxis {
        public float value { get { return Input.GetAxisRaw(name); } }

        public string name;
    }

    [Serializable]
    class MyInputAxisAsButton : IMyInputButton {
        public bool is_held { get; private set; }

        public string name;

        bool is_down;

        [SerializeField] string axis_name;
        [SerializeField] bool is_positive;

        float last_down = float.MinValue;

        public void Update(bool is_paused) {
            float new_axis_value = Input.GetAxisRaw(axis_name);

            is_down = false;
            if ((is_positive && new_axis_value > 0) || (!is_positive && new_axis_value < 0)) {
                if (!is_held) {
                    if (!is_paused) last_down = Time.unscaledTime;
                    is_down = true;
                }
                is_held = true;
            } else {
                is_held = false;
            }
        }

        public bool IsDown() {
            return is_down;
        }
        public bool IsDownBuffered(float buffer_length) {
            bool to_ret = last_down + buffer_length >= Time.unscaledTime;
            last_down = float.MinValue;
            return to_ret;
        }

        public void ClearBuffer() {
            last_down = 0f;
        }
    }
}
