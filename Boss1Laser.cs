using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1Laser : MonoBehaviour
{

    public GameObject laser1;
    public GameObject laser2a;
    public GameObject laser2b;
    GameManager game;

    private bool bossFightStarted;

    void Start()
    {
        laser1.SetActive(false);
        laser2a.SetActive(false);
        laser2b.SetActive(false);
        game = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {

        if (game.InBossFight && !bossFightStarted)
        {
            bossFightStarted = true;
            StartCoroutine(BossLasers());
        }
        if (game.GameOver)
        {
            StopCoroutine(BossLasers());
            laser1.SetActive(false);
            laser2a.SetActive(false);
            laser2b.SetActive(false);
        }
    }

    IEnumerator BossLasers()
    {
        yield return new WaitForSeconds(11);
        laser1.SetActive(true);
        yield return new WaitForSeconds(3);
        laser1.SetActive(false);

        laser2a.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        laser2b.SetActive(true);
        yield return new WaitForSeconds(3);
        laser2a.SetActive(false);
        laser2b.SetActive(false);

        yield return new WaitForSeconds(0.8f);
        bossFightStarted = false;
    }
}
