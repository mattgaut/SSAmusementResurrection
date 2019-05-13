﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour, IInteractable {

    [SerializeField] RoomController home;
    [SerializeField] Teleporter linked_teleporter;
    [SerializeField] Transform teleportation_point;
    [SerializeField] bool open;

    [SerializeField] Animator anim;

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
    }

    void Awake() {
        anim.SetBool("Open", open);
    }

}