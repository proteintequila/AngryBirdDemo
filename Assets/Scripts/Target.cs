using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public int points = 10; // 이 목표물을 맞췄을 때 얻는 점수

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bird"))
        {
            FindObjectOfType<Slingshot>().AddScore(points);
            Destroy(gameObject); // 충돌 후 목표물을 파괴합니다
            FindObjectOfType<Slingshot>().CheckGameClear(); // 오브젝트 파괴 후 게임 클리어 상태 확인
        }
    }
}
