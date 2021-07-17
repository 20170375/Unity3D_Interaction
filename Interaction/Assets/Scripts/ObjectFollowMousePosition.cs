using UnityEngine;

public class ObjectFollowMousePosition : MonoBehaviour
{
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // ȭ���� ���콺 ��ǥ�� �������� ���� ���� ���� ��ǥ�� ���Ѵ�.
        Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
        transform.position = mainCamera.ScreenToWorldPoint(position);
        // y ��ġ�� 0.2f�� ����
        transform.position = new Vector3(transform.position.x, 0.2f, transform.position.z);
    }
}

/*
 * File : ObjectFollowMousePosition.cs
 * Desc
 *  : �ش� ��ũ��Ʈ�� ������ �ִ� ���� ������Ʈ�� ���콺 ��ġ�� �ѾƴٴѴ�.
 *  
 */
