using UnityEngine;
using System.Threading.Tasks;

using System.Collections;
using System.Collections.Generic;
public class cameraPanningScript : MonoBehaviour
{

    public GameObject[] grates;
    private bool shouldPan;
    private int currentGrateIndex = 0;

    public GameObject wall1;
    public GameObject wall2;
    public GameObject wall3;
    public GameObject wall4;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        grates = new GameObject[] { wall1, wall2, wall3, wall4 };
        shouldPan = true;
        currentGrateIndex = 0;

        StartCoroutine(showLevel(grates));
    }

    // Update is called once per frame
    void Update()
    {
    }


    Vector3 startPosition;
    private IEnumerator showLevel(GameObject[] grates) {
        startPosition = transform.position;


        for( int i = 0; i < grates.Length; i++) {

            Vector3 gratePosition = new Vector3(grates[i].transform.position.x, grates[i].transform.position.y, -10);
            yield return StartCoroutine(panTo(gratePosition));
        }

        yield return StartCoroutine(panTo(startPosition));

    }

    public float panSpeed;
    private IEnumerator panTo(Vector3 gratePosition)
    {
        while (Vector3.Distance(transform.position, gratePosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, gratePosition, panSpeed * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(1f);

        transform.position = gratePosition;

    }
}
