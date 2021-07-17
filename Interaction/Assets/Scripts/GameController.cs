using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class GameController : MonoBehaviour
{
    private Person[]     people;

    [Header("Start Options")]
    [SerializeField]
    private GameObject   PersonPrefab;      // Person ������

    [Header("Game Options")]
    [SerializeField]
    private float        affectTerm = 0.5f; // Affect �ֱ�
    [SerializeField]
    private float        affectRate = 0.9f; // Affect Ȯ��

    [Header("UI")]
    [SerializeField]
    private TextMeshProUGUI textRedCnt;
    [SerializeField]
    private TextMeshProUGUI textBlueCnt;
    [SerializeField]
    private TextMeshProUGUI textGreenCnt;
    [SerializeField]
    private GameObject followPrefab;    // ���콺�� ����ٴϴ� �ӽ� ��ü�� ���� ������
    private PersonType personType;      // ������ PersonType
    private bool       isOnButton;      // Button Ȱ��ȭ ����
    private GameObject followClone;     // ���콺�� ����ٴϴ� �ӽ� ��ü
    [SerializeField]
    private Scrollbar  scrollbarRed;
    [SerializeField]
    private Scrollbar  scrollbarBlue;
    [SerializeField]
    private Scrollbar  scrollbarGreen;

    private void Start()
    {
        StartCoroutine("Affect");
    }

    private void Update()
    {
        // ���콺�� UI�� �ӹ��� ���� ���� �Ʒ� �ڵ尡 ������� �ʵ��� ��
        if ( EventSystem.current.IsPointerOverGameObject() == true ) { return; }

        // ���콺 ���� ��ư�� ������ ��
        if ( Input.GetMouseButtonDown(0) )
        {
            int spawnCount = 10;
            
            switch ( personType )
            {
                case PersonType.Red:
                    spawnCount = Mathf.Max(spawnCount, (int)(100 * scrollbarRed.value));
                    break;
                case PersonType.Blue:
                    spawnCount = Mathf.Max(spawnCount, (int)(100 * scrollbarBlue.value));
                    break;
                case PersonType.Green:
                    spawnCount = Mathf.Max(spawnCount, (int)(100 * scrollbarGreen.value));
                    break;
            }

            // Follow �ӽ� ��ü ��ġ�� Spawn
            if ( followClone ) { TrySpawn(followClone.transform.position, spawnCount); }
        }
    }

    private IEnumerator Affect()
    {
        List<Person> reds   = new List<Person>();
        List<Person> blues  = new List<Person>();
        List<Person> greens = new List<Person>();
        Vector3 redPos;
        Vector3 bluePos;
        Vector3 greenPos;
        float distance;
        int redCnt   = 0;
        int blueCnt  = 0;
        int greenCnt = 0;

        while ( true )
        {
            people = FindObjectsOfType<Person>();

            // PersonType�� ���� �з�
            for ( int i=0; i<people.Length; ++i )
            {
                if ( people[i].PersonType == PersonType.Red )      { reds.Add(people[i]); }
                else if (people[i].PersonType == PersonType.Blue)  { blues.Add(people[i]); }
                else if (people[i].PersonType == PersonType.Green) { greens.Add(people[i]); }
            }

            // ������ PersonType �����ġ ���
            redPos   = Vector3.zero;
            bluePos  = Vector3.zero;
            greenPos = Vector3.zero;
            for ( int i=0; i<reds.Count; ++i )   { redPos += reds[i].transform.position; }
            for ( int i=0; i<blues.Count; ++i )  { bluePos += blues[i].transform.position; }
            for ( int i=0; i<greens.Count; ++i ) { greenPos += greens[i].transform.position; }
            redPos   /= reds.Count;
            bluePos  /= blues.Count;
            greenPos /= greens.Count;

            // ������ Person���� ���� ����� ����� PersonType�� �缳��
            for ( int i=0; i<people.Length; ++i )
            {
                // affectRate Ȯ���� PersonType ����
                if ( affectRate < Random.Range(0.0f, 1.0f) )
                {
                    // PersonType ���� Cnt ����
                    if (people[i].PersonType == PersonType.Red) { redCnt++; }
                    else if (people[i].PersonType == PersonType.Blue) { blueCnt++; }
                    else if (people[i].PersonType == PersonType.Green) { greenCnt++; }
                    continue;
                }

                // �� PersonType���� ��ġ ����� ��
                distance = float.MaxValue;
                if ( (reds.Count > 0) && (distance > Vector3.Distance(people[i].transform.position, redPos)) )
                {
                    distance = Vector3.Distance(people[i].transform.position, redPos);
                    people[i].PersonType = PersonType.Red;
                }
                if ( (blues.Count > 0) && (distance > Vector3.Distance(people[i].transform.position, bluePos)) )
                {
                    distance = Vector3.Distance(people[i].transform.position, bluePos);
                    people[i].PersonType = PersonType.Blue;
                }
                if ( (greens.Count > 0) && (distance > Vector3.Distance(people[i].transform.position, greenPos)) )
                {
                    distance = Vector3.Distance(people[i].transform.position, greenPos);
                    people[i].PersonType = PersonType.Green;
                }

                // PersonType ���� Cnt ����
                if (people[i].PersonType == PersonType.Red)        { redCnt++; }
                else if (people[i].PersonType == PersonType.Blue)  { blueCnt++; }
                else if (people[i].PersonType == PersonType.Green) { greenCnt++; }
            }

            // Text UI ����
            textRedCnt.text   = "x " + redCnt;
            textBlueCnt.text  = "x " + blueCnt;
            textGreenCnt.text = "x " + greenCnt;

            // Debug
            //print("redPos: " + redPos + " bluePos: " + bluePos + " greenPos: " + greenPos);
            if ( reds.Count > 0 )   { Debug.DrawRay(redPos, Vector3.up, Color.red, affectTerm); }
            if ( blues.Count > 0 )  { Debug.DrawRay(bluePos, Vector3.up, Color.blue, affectTerm); }
            if ( greens.Count > 0 ) { Debug.DrawRay(greenPos, Vector3.up, Color.green, affectTerm); }

            // ���� �ʱ�ȭ
            reds.Clear();
            blues.Clear();
            greens.Clear();
            redCnt   = 0;
            blueCnt  = 0;
            greenCnt = 0;

            yield return new WaitForSeconds(affectTerm);
        }
    }

    private void Finish()
    {
        StopCoroutine("Affect");

        // ��� Person ����
        people = FindObjectsOfType<Person>();
        foreach ( Person person in people ) { person.Stop(); }

        print("Finish");
    }

    private void SpawnPerson(Vector3 position, PersonType personType, int count)
    {
        for ( int i=0; i<count; ++i )
        {
            GameObject clone = Instantiate(PersonPrefab, position, Quaternion.identity);
            clone.GetComponent<Person>().PersonType = personType;
        }
    }

    private void TrySpawn(Vector3 position, int count)
    {
        // isOnButton�� true�� ��쿡�� ����
        if (isOnButton == true)
        {
            SpawnPerson(position, personType, count);
        }
    }

    private IEnumerator CancelSpawn()
    {
        while ( true )
        {
            // ESCŰ �Ǵ� ���콺 ������ ��ư�� ������ �� Ÿ�� �Ǽ� ���
            if ( Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1) )
            {
                isOnButton = false;
                // ���콺�� ����ٴϴ� �ӽ� Ÿ�� ����
                Destroy(followClone);
                break;
            }

            yield return null;
        }
    }

    public void ReadyToSpawn(int personType)
    {
        this.personType = (PersonType)personType;

        isOnButton = true;
        // ���콺�� ����ٴϴ� �ӽ� ��ü ����

        if ( !followClone ) { followClone = Instantiate(followPrefab); }
        switch ( this.personType )
        {
            case PersonType.Red:
                followClone.GetComponent<SpriteRenderer>().color = Color.red;
                break;
            case PersonType.Blue:
                followClone.GetComponent<SpriteRenderer>().color = Color.blue;
                break;
            case PersonType.Green:
                followClone.GetComponent<SpriteRenderer>().color = Color.green;
                break;
        }
        
        // Spawn ��Ҹ� ���� �ڷ�ƾ ����
        StartCoroutine("CancelSpawn");
    }
}
