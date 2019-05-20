using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Teleporter : MonoBehaviour, IInteractable {

    public UnityEvent arrival_event {
        get { return on_arrival; }
    }
    public UnityEvent departure_event {
        get { return on_departure; }
    }

    public bool is_available { get { return !teleporting && open; } }

    [SerializeField] RoomController home;
    [SerializeField] Teleporter linked_teleporter;
    [SerializeField] Transform teleportation_point;
    [SerializeField] bool open;

    [SerializeField] Animator anim;

    [SerializeField] UnityEvent on_arrival, on_departure;

    bool teleporting;
    float cutscene_length = 2f;

    public void Interact(Player player) {
        if (!teleporting && open) {
            StartCoroutine(TeleportRoutine(player, linked_teleporter));
        }
    }

    public void Link(Teleporter teleporter) {
        linked_teleporter = teleporter;
        teleporter.linked_teleporter = this;
    }

    public void SetHighlight(bool is_highlighted) {

    }

    public void SetOpen(bool open) {
        this.open = open;

        anim.SetBool("Open", open);
    }

    private IEnumerator TeleportRoutine(Player player, Teleporter teleport_to) {
        departure_event.Invoke();

        float length = cutscene_length;
        GameManager.instance.StartCutscene();

        teleport_to.teleporting = true;
        teleporting = true;

        yield return UIHandler.FadeToBlack(length / 2f);

        if (home != teleport_to.home) RoomManager.instance.SetActiveRoom(teleport_to.home, false);
        player.transform.position = teleport_to.teleportation_point.transform.position;

        yield return UIHandler.FadeInFromBlack(length / 2f);

        teleport_to.teleporting = false;
        teleporting = false;

        GameManager.instance.EndCutscene();

        teleport_to.arrival_event.Invoke();
    }

    void Awake() {
        anim.SetBool("Open", open);
    }

}
