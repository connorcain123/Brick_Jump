using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGen : MonoBehaviour
{
    public GameObject platform, player;

    //Max length of platform
    private int maxPlatLen = 10;
    //Min length of platform
    private int minPlatLen = 3;
    //Previous platforms Y value 
    private int platHeight;
    //Prev platform right edge
    private int prevPlatEdge;
    //Max height increase of next platform
    private int maxHeight = 2;
    //Max height decrease of next platform
    private int maxDrop = -3;
    //Max gap between platforms to still be playable
    private int maxGap = 2;

    private int startPlatAmount = 3;


    private GameObject obj, deathCollider;
    private float rightEdge;
    private int platSize;

    List<Vector2> colliderPoints;
    List<GameObject> platforms;

    // Start is called before the first frame update
    void Start()
    {
        platforms = new List<GameObject>();

        //Creates first platform of level
        obj = Instantiate(platform, new Vector3(1f, 0, 0), Quaternion.identity);      
        rightEdge = obj.transform.position.x + (obj.transform.localScale.x / 2);
        platforms.Add(obj);

        colliderPoints = new List<Vector2>();
        colliderPoints.Add(new Vector2(0, -5));

        for (int i = 0; i <= startPlatAmount; i++)
        {
            platSize = Mathf.RoundToInt(Random.Range(minPlatLen, maxPlatLen));
            platHeight = platHeight + Mathf.RoundToInt(Random.Range(maxDrop, maxHeight));
            obj = Instantiate(platform, new Vector3(rightEdge + platSize + Mathf.RoundToInt(Random.Range(1, maxGap)), platHeight, 0), Quaternion.identity);
            obj.transform.localScale = new Vector3(platSize, 1, 1);
            rightEdge = obj.transform.position.x + (obj.transform.localScale.x / 2);
            colliderPoints.Add(new Vector2(obj.transform.position.x, obj.transform.position.y - 5));
            platforms.Add(obj);
        }

        CreateDeathCollider();

        StartCoroutine(InfiniteGeneration());
    }

    private void CreateDeathCollider()
    {
        deathCollider = new GameObject();
        deathCollider.AddComponent<EdgeCollider2D>();
        deathCollider.tag = "deathCollider";
        EdgeCollider2D col = deathCollider.GetComponent<EdgeCollider2D>();
        col.points = colliderPoints.ToArray();
    }

    private void UpdateDeathCollider()
    {
        EdgeCollider2D col = deathCollider.GetComponent<EdgeCollider2D>();
        col.points = colliderPoints.ToArray();
    }

    private IEnumerator InfiniteGeneration()
    {
        while (true)
        {
            platSize = Mathf.RoundToInt(Random.Range(minPlatLen, maxPlatLen));
            platHeight = platHeight + Mathf.RoundToInt(Random.Range(maxDrop, maxHeight));
            obj = Instantiate(platform, new Vector3(rightEdge + platSize + Mathf.RoundToInt(Random.Range(1, maxGap)), platHeight, 0), Quaternion.identity);
            obj.transform.localScale = new Vector3(platSize, 1, 1);
            platforms.Add(obj);
            rightEdge = obj.transform.position.x + (obj.transform.localScale.x / 2);
            colliderPoints.Add(new Vector2(obj.transform.position.x, obj.transform.position.y - 5));
            UpdateDeathCollider();

            for (int i = platforms.Count - 1; i >= 0; i--)
            {
                if (platforms[i].transform.position.x < (player.transform.position.x - 7))
                {
                    Destroy(platforms[i]);
                    platforms.RemoveAt(i);
                    colliderPoints.RemoveAt(i);
                }
                    
            }

            yield return new WaitForSeconds(1.25f);
        }
    }
}
