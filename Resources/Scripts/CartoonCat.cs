using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CartoonCat : MonoBehaviour
{
    public int level;

    public bool audioActive;
    public bool closeToPlayer;

    public bool followPlayer;
    public NavMeshAgent nav;

    public float distanceToPlayer;

    public AudioSource primaryAudio;
    public AudioDistortionFilter audioDist;
    public AudioEchoFilter audioEch;

    public GameObject originLocat;
    public GameObject playerRespawnLocat;
    public GameObject entireCat;

    public Transform raycastOrigin;

    public Transform catHead;

    public bool seesPlayer;

    private bool searching;

    public List<GameObject> allPatrolPoints = new List<GameObject>();

    private GameObject currentPoint;
    private GameObject movingToPoint;

    public List<Transform> SpawnPoints = new List<Transform>();

    public enum AItypeData
    {
        Patrol,
        Chase
    };

    public AItypeData currentaiType;

    public EnableTrigger triggerOnKill;

    private GameObject Player;

    private void Start()
    {
        if (GameObject.FindWithTag("Player") != null)
        {
            Player = GameObject.FindWithTag("Player");
        }
    }

    public GameObject FindNearestPoint()
    {
        PatrolPoint pointClosest = null;

        foreach (GameObject point in allPatrolPoints)
        {
            if (pointClosest == null)
            {
                pointClosest = point.GetComponent<PatrolPoint>();
            }
            else
            {
                if (point.GetComponent<PatrolPoint>().distanceFromPlayer < pointClosest.distanceFromPlayer)
                {
                    pointClosest = point.GetComponent<PatrolPoint>();
                }
            }
        }

        return pointClosest.gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (followPlayer && other.tag == "Patrol")
        {
            PatrolPoint newPoint = other.gameObject.GetComponent<PatrolPoint>();
            movingToPoint = newPoint.CalculateNextPoint(currentPoint);
            currentPoint = other.gameObject;
        }
    }

    public void ClearPatrolData()
    {
        currentaiType = AItypeData.Patrol;
        currentPoint = null;
        movingToPoint = null;
    }

    private void FixedUpdate()
    {
        if (!followPlayer)
        {
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(raycastOrigin.transform.position, raycastOrigin.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            if (hit.collider.tag == "Player")
            {
                Debug.DrawRay(raycastOrigin.transform.position, raycastOrigin.transform.TransformDirection(Vector3.forward) * 1000, Color.green);
                //print("HIT PLAYER");
                seesPlayer = true;

                if (!searching)
                {
                    StartCoroutine(WaitCheck());
                }
            }
            else
            {
                Debug.DrawRay(raycastOrigin.transform.position, raycastOrigin.transform.TransformDirection(Vector3.forward) * 1000, Color.red);
                //Debug.Log("Did not Hit");
                seesPlayer = false;
            }
        }
        else
        {
            Debug.DrawRay(raycastOrigin.transform.position, raycastOrigin.transform.TransformDirection(Vector3.forward) * 1000, Color.red);
            //Debug.Log("Did not Hit");
            seesPlayer = false;
        }

        if (seesPlayer)
        {
            catHead.GetComponent<FacePlayer>().enabled = true;
        }
        else
        {
            catHead.GetComponent<FacePlayer>().enabled = false;
            catHead.transform.rotation = new Quaternion();
        }
        
    }

    IEnumerator WaitCheck()
    {
        searching = true;
        currentaiType = AItypeData.Chase;
        yield return new WaitForSeconds(5.0f);
        if (!seesPlayer)
        {
            currentaiType = AItypeData.Patrol;
            searching = false;
        }
        else
        {
            StartCoroutine(WaitCheck());
        }
    }

    private void Update()
    {
        if (followPlayer)
        {

            if (currentaiType == AItypeData.Chase)
            {
                nav.speed = 4.25f;
                if (nav != null)
                {
                    nav.SetDestination(GameObject.FindWithTag("Player").transform.position);
                }
            }
            else
            {
                nav.speed = 3.5f;
                if (movingToPoint == null)
                {
                    movingToPoint = FindNearestPoint();
                }

                nav.SetDestination(movingToPoint.transform.position);
            }
        }

        if (primaryAudio != null)
        {
            if (audioActive)
            {
                if (!primaryAudio.enabled)
                {
                    primaryAudio.enabled = true;
                }
            }
            else
            {
                if (primaryAudio.enabled)
                {
                    primaryAudio.enabled = false;
                }
            }
        }

        if (Player != null)
        {
            distanceToPlayer = Vector3.Distance(this.transform.position, Player.transform.position);

            if (distanceToPlayer <= 10.0f)
            {
                closeToPlayer = true;
            }
            else
            {
                closeToPlayer = false;
            }

            float distortion = Mathf.Pow(distanceToPlayer, -2) * 20;

            if (distortion > .8f)
            {
                distortion = 0.8f;
            }

            if (distanceToPlayer < 12.5f)
            {
                Player.GetComponent<PlayerInventory>().flashlightObj.GetComponent<Flashlight>().light.GetComponent<FlickeringLight>().enabled = true;
            }
            else
            {
                Player.GetComponent<PlayerInventory>().flashlightObj.GetComponent<Flashlight>().light.GetComponent<FlickeringLight>().enabled = false;
                Player.GetComponent<PlayerInventory>().flashlightObj.GetComponent<Flashlight>().light.GetComponent<Light>().intensity = 1000000;
                Player.GetComponent<PlayerInventory>().flashlightObj.GetComponent<Flashlight>().light.GetComponent<Light>().range = 60.48f;
            }

            if (distanceToPlayer < 2.0f)
            {
                if (level == 2)
                {
                    return;
                }
                //Kill the player
                Player.GetComponent<Effects>().Scare(2);

                Player.transform.position = playerRespawnLocat.transform.position;
                Player.transform.rotation = playerRespawnLocat.transform.rotation;

                SetCatPositionToFurthest(Player.transform);

                Player.GetComponent<PlayerInventory>().flashlightObj.GetComponent<Flashlight>().light.GetComponent<FlickeringLight>().enabled = false;

                if (triggerOnKill != null && level == 0)
                {
                    triggerOnKill.Trigger();
                }

                //entireCat.gameObject.SetActive(false);
            }

            audioDist.distortionLevel = distortion;
        }
    }

    public void SetCatPositionToFurthest(Transform playerLocat)
    {
        float currentFurthest = 0;
        Transform currentSpawn = null;

        foreach (Transform pos in SpawnPoints)
        {
            if (currentSpawn == null)
            {
                currentSpawn = pos;
            }

            if (Vector3.Distance(pos.position, playerLocat.position) > currentFurthest)
            {
                currentFurthest = Vector3.Distance(pos.position, playerLocat.position);
                currentSpawn = pos;
            }
        }

        entireCat.transform.position = currentSpawn.transform.position;
        entireCat.transform.rotation = currentSpawn.transform.rotation;
    }
}
