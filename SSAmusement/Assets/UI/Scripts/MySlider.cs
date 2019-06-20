using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MySlider : MonoBehaviour {
    [SerializeField] RectTransform target;
    [SerializeField] RectTransform background;
    [SerializeField] Text target_text;

    bool set_text;

    [SerializeField]
    bool x_axis, y_axis;
    float width, height;

    [SerializeField]
    [Range(0f, 1f)]
    float start_fill;

    float _fill;
    public float fill {
        get { return _fill; }
        protected set {
            _fill = value;
            if (_fill > 1) {
                _fill = 1;
            } else if (_fill < 0) {
                _fill = 0;
            }
            SetImage();
        }
    }

    public void SetFill(float percent) {
        fill = percent;

        SetText(Mathf.RoundToInt(percent) + "%");
    }

    public void SetFill(float percent, string text) {
        fill = percent;
        SetText(text);
    }

    public void SetFill(float over, float under) {
        if (under == 0) fill = 0;
        else fill = (over / under);

        SetText(Mathf.RoundToInt(over) + " / " + Mathf.RoundToInt(under));
    }
    public void SetFill(float over, float under, string text) {
        if (under == 0) fill = 0;
        else fill = (over / under);
        SetText(text);
    }

    public void SetText(string text) {
        if (target_text) {
            target_text.text = text;
        }
    }

    protected void Awake() {
        Init();
    }

    protected virtual void Init() {
        target = target.GetComponent<RectTransform>();
        background = background.GetComponent<RectTransform>();
        target.sizeDelta = Vector2.zero;
        width = background.rect.width;
        height = background.rect.height;
        fill = start_fill;
    }

    protected virtual void SetImage() {
        target.transform.localScale = new Vector3(x_axis ? fill : 1, y_axis ? fill : 1, 0);
        target.localPosition = new Vector3(x_axis ? -(width / 2) * (1 - fill) : 0, y_axis ? -(height / 2) * (1 - fill) : 0, 0) + background.localPosition;
    }
}
