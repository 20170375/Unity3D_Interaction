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
    private GameObject   PersonPrefab;      // Person 프리팹

    [Header("Game Options")]
    [SerializeField]
    private float        affectTerm = 0.5f; // Affect 주기
    [SerializeField]
    private float        affectRate = 0.9f; // Affect 확률

    [Header("UI")]
    [SerializeField]
    private TextMeshProUGUI textRedCnt;
    [SerializeField]
    private TextMeshProUGUI textBlueCnt;
    [SerializeField]
    private TextMeshProUGUI textGreenCnt;
    [SerializeField]
    private GameObject followPrefab;    // 마우스를 따라다니는 임시 객체를 위한 프리팹
    private PersonType personType;      // 생성할 PersonType
    private bool       isOnButton;      // Button 활성화 여부
    private GameObject followClone;     // 마우스를 따라다니는 임시 객체
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
        // 마우스가 UI에 머물러 있을 때는 아래 코드가 실행되지 않도록 함
        if ( EventSystem.current.IsPointerOverGameObject() == true ) { return; }

        // 마우스 왼쪽 버튼을 눌렀을 때
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

            // Follow 임시 객체 위치에 Spawn
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

            // PersonType에 따라 분류
            for ( int i=0; i<people.Length; ++i )
            {
                if ( people[i].PersonType == PersonType.Red )      { reds.Add(people[i]); }
                else if (people[i].PersonType == PersonType.Blue)  { blues.Add(people[i]); }
                else if (people[i].PersonType == PersonType.Green) { greens.Add(people[i]); }
            }

            // 각각의 PersonType 평균위치 계산
            redPos   = Vector3.zero;
            bluePos  = Vector3.zero;
            greenPos = Vector3.zero;
            for ( int i=0; i<reds.Count; ++i )   { redPos += reds[i].transform.position; }
            for ( int i=0; i<blues.Count; ++i )  { bluePos += blues[i].transform.position; }
            for ( int i=0; i<greens.Count; ++i ) { greenPos += greens[i].transform.position; }
            redPos   /= reds.Count;
            bluePos  /= blues.Count;
            greenPos /= greens.Count;

            // 각각의 Person마다 가장 가까운 평균의 PersonType로 재설정
            for ( int i=0; i<people.Length; ++i )
            {
                // affectRate 확률로 PersonType 변경
                if ( affectRate < Random.Range(0.0f, 1.0f) )
                {
                    // PersonType 마다 Cnt 갱신
                    if (people[i].PersonType == PersonType.Red) { redCnt++; }
                    else if (people[i].PersonType == PersonType.Blue) { blueCnt++; }
                    else if (people[i].PersonType == PersonType.Green) { greenCnt++; }
                    continue;
                }

                // 각 PersonType마다 위치 평균을 비교
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

                // PersonType 마다 Cnt 갱신
                if (people[i].PersonType == PersonType.Red)        { redCnt++; }
                else if (people[i].PersonType == PersonType.Blue)  { blueCnt++; }
                else if (people[i].PersonType == PersonType.Green) { greenCnt++; }
            }

            // Text UI 갱신
            textRedCnt.text   = "x " + redCnt;
            textBlueCnt.text  = "x " + blueCnt;
            textGreenCnt.text = "x " + greenCnt;

            // Debug
            //print("redPos: " + redPos + " bluePos: " + bluePos + " greenPos: " + greenPos);
            if ( reds.Count > 0 )   { Debug.DrawRay(redPos, Vector3.up, Color.red, affectTerm); }
            if ( blues.Count > 0 )  { Debug.DrawRay(bluePos, Vector3.up, Color.blue, affectTerm); }
            if ( greens.Count > 0 ) { Debug.DrawRay(greenPos, Vector3.up, Color.green, affectTerm); }

            // 변수 초기화
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

        // 모든 Person 정지
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
        // isOnButton이 true일 경우에만 실행
        if (isOnButton == true)
        {
            SpawnPerson(position, personType, count);
        }
    }

    private IEnumerator CancelSpawn()
    {
        while ( true )
        {
            // ESC키 또는 마우스 오른쪽 버튼을 눌렀을 때 타워 건설 취소
            if ( Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1) )
            {
                isOnButton = false;
                // 마우스를 따라다니는 임시 타워 삭제
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
        // 마우스를 따라다니는 임시 객체 생성

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
        
        // Spawn 취소를 위한 코루틴 실행
        StartCoroutine("CancelSpawn");
    }
}
