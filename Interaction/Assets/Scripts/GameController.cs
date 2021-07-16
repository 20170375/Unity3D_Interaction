using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    private Person[]     people;

    [Header("Start Options")]
    [SerializeField]
    private GameObject   PersonPrefab;      // Person ������
    [SerializeField]
    private int          redCount   = 40;   // ���۽� Red ��
    [SerializeField]
    private int          blueCount  = 40;   // ���۽� Blue ��
    [SerializeField]
    private int          greenCount = 40;   // ���۽� Green ��

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

    private void Awake()
    {
        Vector3 redPos   = new Vector3(9, 0.2f, 4);
        Vector3 bluePos  = new Vector3(-9, 0.2f, -4);
        Vector3 greenPos = new Vector3(-9, 0.2f, 4);
        SpawnPerson(redPos, PersonType.Red, redCount);
        SpawnPerson(bluePos, PersonType.Blue, blueCount);
        SpawnPerson(greenPos, PersonType.Green, greenCount);
    }

    private void Start()
    {
        StartCoroutine("Affect");
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
        int result   = 0;

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

            // �� PersonType�� ���Ҵٸ�
            result += (reds.Count == 0) ? 1 : 0;
            result += (blues.Count == 0) ? 1 : 0;
            result += (greens.Count == 0) ? 1 : 0;
            if ( result == 1 ) { Finish(); }

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
            result   = 0;

            yield return new WaitForSeconds(affectTerm);
        }
    }

    private void SpawnPerson(Vector3 position, PersonType personType, int count)
    {
        for ( int i=0; i<count; ++i )
        {
            GameObject clone = Instantiate(PersonPrefab, position, Quaternion.identity);
            clone.GetComponent<Person>().PersonType = personType;
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
}
