using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotallyUsefullAiCompnent : MonoBehaviour {

    Vector3[] raystarts = new Vector3[] { new Vector3(0, 0.3f, -2f), new Vector3(0, 1f, -2f), new Vector3(1, 0.3f, -2f), new Vector3(1, 1f, -2f), new Vector3(-1, 0.3f, -2f), new Vector3(-1, 1f, -2f) };

    public Player player;
    public WorldManager worldManager;

    int currentPos;

    int possiblePosition = -2;
    int jumpIndex = -1;

    bool correctPosition = false;

    void Update () {
        currentPos = player.CurrentPosition;

        SearchPassage();
        Action();

        if (jumpIndex >= 0) {
            Vector3 rayposition = raystarts[jumpIndex];
            Ray ray = new Ray(rayposition + new Vector3(0, 0, 2), Vector3.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 6 * worldManager.worldSpeedMultiplier, LayerMask.GetMask("AI"))) {
                if (hit.collider != null) {
                    if (hit.distance < 1.5f + ((worldManager.worldSpeedMultiplier - 1) * 0.1f)) {
                        player.jump = true;
                    } else {
                        player.jump = false;
                    }
                }
            } else {
                player.jump = false;
            }
        } else {
            player.jump = false;
        }
    }

    /// <summary>
    /// Find a passage in the world
    /// </summary>
    void SearchPassage() {
        int safeIndex = 0;

        jumpIndex = -1;

        for (int i = 0; i < 6; i++) {
            Vector3 rayposition = raystarts[i];
            Ray ray = new Ray(rayposition, Vector3.forward);
            RaycastHit hit;
            
            if (!Physics.Raycast(ray, out hit, 6f, LayerMask.GetMask("AI"))){
                //Debug.DrawRay(ray.origin, ray.direction * 6f, Color.black, 0.1f);
                safeIndex = i;
                break;
            }
        }

        int temp = Mathf.FloorToInt((safeIndex / 2) - 1);

        possiblePosition = temp + 1 == 2 ? -1 : temp + 1;

        if (safeIndex % 2 == 1) {
            jumpIndex = Mathf.FloorToInt(safeIndex / 2);
        }

        correctPosition = possiblePosition == currentPos;
    }

    /// <summary>
    /// Go to the right position
    /// </summary>
    void Action() {
        if (!correctPosition && currentPos > possiblePosition) {
            player.left = true;
        } else {
            player.left = false;
        }

        if (!correctPosition && currentPos < possiblePosition) {
            player.right = true;
        }
        else {
            player.right = false;
        }
    }
}
