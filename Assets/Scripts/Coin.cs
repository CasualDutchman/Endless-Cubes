using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour {

    Transform rotator;

    public AnimationCurve curve;

    float timer;

    Player player;

	void Start () {
        rotator = transform.GetChild(0);
        rotator.localEulerAngles = new Vector3(0, Random.Range(0, 180), 0);
        timer = Random.Range(0.00f, 1.00f);

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
	}
	
	void Update () {
        if (player.Alive) { //go up and down and rotate when player is alive
            timer += Time.deltaTime;
            if (timer >= 1)
                timer -= 1;

            rotator.localPosition = new Vector3(0, curve.Evaluate(timer) * 0.2f, 0);

            rotator.Rotate(Vector3.up * 150 * Time.deltaTime);
        }
	}
}
