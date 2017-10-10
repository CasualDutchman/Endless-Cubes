using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldManager : MonoBehaviour {

    Player player;

    List<GameObject> segments = new List<GameObject>();
    List<Transform> currentSegments = new List<Transform>();

    //lists of index for coins and obstacles, needed for alternating pattern
    List<int> obstacleIndexes = new List<int>();
    List<int> coinIndexes = new List<int>();

    [Header("UI Components")]
    public Text travelledText;
    public Text speedText;

    [Header("World var")]
    public int startSegmentAmount = 6;
    public float segmentSpeed = 3;
    float originalSegmentSpeed;

    int lastSegment;

    float metersTravelled;

    float highestSpeed;

	void Start () {
        Application.runInBackground = true;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        originalSegmentSpeed = segmentSpeed;

        int i = 0;
        Object[] resources = Resources.LoadAll("Segments", typeof(GameObject));
        foreach (Object obj in resources) {
            segments.Add((GameObject)obj);

            if (obj.name.EndsWith("o"))
                obstacleIndexes.Add(i);

            if (obj.name.EndsWith("c"))
                coinIndexes.Add(i);

            i++;
        }

        Reset();
    }
	
    /// <summary>
    /// Reset the world
    /// </summary>
    public void Reset() {
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
        currentSegments.Clear();

        segmentSpeed = originalSegmentSpeed;
        metersTravelled = 0;

        for (int i = 0; i < startSegmentAmount; i++) {
            AddSegment(new Vector3(0, 0, -10 + (i * 10)), i < startSegmentAmount - 1);
        }

        travelledText.transform.parent.gameObject.SetActive(GetComponent<TotallyUsefullAiCompnent>().enabled);
        speedText.transform.parent.gameObject.SetActive(GetComponent<TotallyUsefullAiCompnent>().enabled);
    }

	void Update () {
        if (player.Alive) {
            segmentSpeed += Time.deltaTime * 0.01f;

            metersTravelled += Time.deltaTime * segmentSpeed;

            if (GetComponent<TotallyUsefullAiCompnent>().enabled) {
                travelledText.text = metersTravelled.ToString("F1");
                speedText.text = (segmentSpeed * 3.6f).ToString("F1");
            }

            if (segmentSpeed * 3.6f > highestSpeed)
                highestSpeed = segmentSpeed * 3.6f;

            foreach (Transform trans in currentSegments) {
                trans.Translate(-Vector3.forward * Time.deltaTime * segmentSpeed);
            }

            if (currentSegments[0].position.z < -13) {
                Transform temp = currentSegments[0];
                currentSegments.Remove(temp);
                Destroy(temp.gameObject);

                AddSegment(currentSegments[currentSegments.Count - 1].position + new Vector3(0, 0, 10), false);
            }
        }
	}

    /// <summary>
    /// Get next segment based on previous segment
    /// </summary>
    GameObject GetSegment() {
        int index = 1;

        if (coinIndexes.Contains(lastSegment)) {
            index = obstacleIndexes[Random.Range(0, obstacleIndexes.Count)];
        }
        else{
            index = coinIndexes[Random.Range(0, coinIndexes.Count)];
        }

        lastSegment = index;
        return segments[index];
    }

    /// <summary>
    /// Stop the world from moving
    /// </summary>
    public void StopWorld() {
        segmentSpeed = 0;
    }

    /// <summary>
    /// Add a segment to the world
    /// </summary>
    void AddSegment(Vector3 position, bool begin = true) {
        GameObject go = Instantiate(begin ? segments[0] : GetSegment(), transform);
        go.transform.position = position;
        currentSegments.Add(go.transform);
    }

    /// <summary>
    /// Quit the game
    /// </summary>
    public void QuitGame() {
        Application.Quit();
    }

    /// <summary>
    /// Multiplier to match speed of world
    /// </summary>
    public float worldSpeedMultiplier {
        get {
            return segmentSpeed / originalSegmentSpeed;
        }
    }

    /// <summary>
    /// Player in WorldManager
    /// </summary>
    public Player GetPlayer {
        get {
            return player;
        }
    }

    /// <summary>
    /// Get meters travelled
    /// </summary>
    public float TravelledMeters {
        get {
            return metersTravelled;
        }
    }

    /// <summary>
    /// Get highest speed
    /// </summary>
    public float HighSpeed {
        get {
            return highestSpeed;
        }
    }
}
