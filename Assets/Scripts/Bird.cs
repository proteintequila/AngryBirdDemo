using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // Ground와 충돌했는지 확인
        {
            Debug.Log("Bird collided with Ground"); // Bird가 Ground에 닿았을 때 콘솔에 출력
            Invoke("DestroyBird", 2); // 새가 Ground와 충돌한 후 2초 뒤에 삭제
        }
    }

    void DestroyBird()
    {
        Destroy(gameObject); // 새 오브젝트 삭제
    }
}

