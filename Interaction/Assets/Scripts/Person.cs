using System.Collections;
using UnityEngine;

public enum PersonType { Red=0, Blue, Green, }

public class Person : MonoBehaviour
{
    private Movement3D   movement3D;
    private MeshRenderer meshRenderer;

    [SerializeField]
    private Color[]      colors;
    [SerializeField]
    private PersonType   personType = PersonType.Red;

    [SerializeField]
    private float        moveTerm = 0.3f;

    public PersonType    PersonType
    {
        set => personType = value;
        get => personType;
    }

    private void Awake()
    {
        movement3D   = GetComponent<Movement3D>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        StartCoroutine("OnMove");
    }

    private void Update()
    {
        meshRenderer.material.color = colors[(int)personType];
    }

    private IEnumerator OnMove()
    {
        while ( true )
        {
            Vector3 randPos = Random.insideUnitSphere.normalized;
            randPos.y = 0;

            // 위치 설정 : 현위치 + (랜덤).정규화
            Vector3 goalPos = transform.position + randPos;

            // 오브젝트 이동
            movement3D.MoveTo(goalPos);

            yield return new WaitForSeconds(moveTerm);
        }
    }

    public void Stop()
    {
        StopCoroutine("OnMove");
    }
}
