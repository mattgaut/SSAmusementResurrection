using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(Image))]
public class MapObject : MonoBehaviour {

    public Vector3 center { get; private set; }

    float width, height;
    float room_icon_scale = 2f / 3f;

    RectTransform rt;
    Image image;

    [SerializeField] Sprite active_sprite, inactive_sprite;
    [SerializeField] Image room_icon_image;

    public void Awake() {
        rt = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    public void SetToRoom(Room r, float scale) {
        width = r.size.x;
        height = r.size.y;
        center = (r.position + (r.size - Vector2.one) / 2f) * scale;
        transform.localPosition = center;
        rt.sizeDelta = new Vector2(width * scale, height * scale);
        room_icon_image.rectTransform.sizeDelta = new Vector2(scale, scale) * room_icon_scale;
    }

    public void SetSprite(bool active) {
        if (active) {
            image.sprite = active_sprite;
        } else {
            image.sprite = inactive_sprite;
        }
    }

    public void SetRoomIcon(Sprite sprite) {
        room_icon_image.enabled = (sprite != null);
        room_icon_image.sprite = sprite;
    }
}
