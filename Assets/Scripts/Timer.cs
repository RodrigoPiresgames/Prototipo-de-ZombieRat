using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{

    [SerializeField] private float  timeMax = 200.0f;
    [SerializeField] private Image bar;

    private float timeLeft;
    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        timeLeft = timeMax;
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;

            bar.fillAmount = timeLeft / timeMax;

            if (timeLeft <= 0)
            {
                bar.fillAmount = 0;
                player.Die();
            }
        }
    }
}
