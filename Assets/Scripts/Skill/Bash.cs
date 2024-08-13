using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

[RequireComponent(typeof(Player))]
public class Bash : MonoBehaviour
{
    private Rigidbody2D rigidbody2d;
    private Collider2D collider2d;
    private LayerMask layerMask;
    private Player player;
    private Arrow arrow;
    private float dashForce = 10f;
    private BashState bashState;
    private bool isDuring;
    private bool isRelease;
    private float aimCountdown;
    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        player = GetComponent<Player>();
        arrow = GetComponent<Arrow>();
        bashState = BashState.None;
    }

    private void OnEnable()
    {
        player.bashEvent.OnBash += OnBashEvent_OnBash;

    }
    private void OnDisable()
    {

        player.bashEvent.OnBash -= OnBashEvent_OnBash;
    }

    private void OnBashEvent_OnBash(BashEvent bashEvent, BashEventArgs bashEventArgs)
    {
        bashState = bashEventArgs.bashState;
    }
    private void Start()
    {
        layerMask = LayerMask.GetMask("EnemyAmmo");
    }
    private void Update()
    {
        switch (bashState)
        {
            case BashState.ActiveBash:
                Prepare();
                break;
            case BashState.DuringBash:
                if (isDuring)
                    During();
                break;
            case BashState.ReleaseBash:
                if (isRelease)
                    Release();
                break;
            case BashState.None:
                InitializeFlagVarible();
                break;
        }
    }

    private void Prepare()
    {
        collider2d = Physics2D.OverlapCircle(transform.position, 2f, layerMask.value);
        if (collider2d != null)
        {
            Time.timeScale = 0f;
            isDuring = true;
            aimCountdown = 10f;
        }
    }

    private void During()
    {
        aimCountdown -= Time.unscaledDeltaTime;
        if (aimCountdown <= 0)
        {
            Time.timeScale = 1f;
            bashState = BashState.None;
        }
        isRelease = true;
        if (collider2d != null)
            arrow.MakeArrow(HelperUtilities.GetMouseWorldPosition(), collider2d.transform.position);
    }

    private void Release()
    {
        Time.timeScale = 1f;
        if (collider2d != null && isRelease)
        {
            collider2d.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            collider2d.gameObject.layer = LayerMask.NameToLayer("BashAmmo");
            collider2d.GetComponent<Rigidbody2D>().AddForce(((Vector2)collider2d.transform.position - (Vector2)HelperUtilities.GetMouseWorldPosition()).normalized * dashForce, ForceMode2D.Impulse);
            bashState = BashState.None;
            arrow.ClearArrow();
        }
    }

    private void InitializeFlagVarible()
    {
        isDuring = false;
        isRelease = false;
    }
}


