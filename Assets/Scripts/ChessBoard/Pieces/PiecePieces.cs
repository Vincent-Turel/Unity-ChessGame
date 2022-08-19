using System.Collections;
using Exploder.Utils;
using UnityEngine;
using UnityEngine.AI;

public class PiecePieces : MonoBehaviour
{
    //public Placement tilePlacement{ get; set; }
    public bool IsWhite { get; set;}
    
    private Animator _anim;
    private NavMeshAgent _navMeshAgent;
    private PieceManager _pieceManager;

    private AudioSource _audioWalk;
    private AudioSource _audioAttack;
    private AudioSource _audioGetHit;
    
    private bool _moving;
    private bool _attacking;
    private bool _attackAnimation;
    private bool _rock;
    public bool _arrived { get; private set; }

    private Vector3 attackDestination;
    private GameObject _enemy;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        _pieceManager = GetComponentInParent<PieceManager>();

        var audioSources = GetComponentsInChildren<AudioSource>();
        foreach (var audioSource in audioSources)
        {
            audioSource.volume = 0.5f;
            audioSource.playOnAwake = false;
        }
        _audioWalk = audioSources[0];
        _audioWalk.loop = true;
        _audioAttack= audioSources[1];
        _audioGetHit= audioSources[2];
    }

    public void ResetMovement()
    {
        _moving = false;
        _attacking = false;
        Rotate();
    }

    public void Move(Vector3 placement, bool rock = false)
    {
        _arrived = false;
        _navMeshAgent.destination = placement;
        _moving = true;
        _audioWalk.Play();
        _rock = rock;
    }

    public void Attack(Vector3 placement, Vector3 enemyPlacement, GameObject enemy)
    {
        Move(enemyPlacement);
        attackDestination = placement;
        _attacking = true;
        _enemy = enemy;
    }

    private IEnumerator AttackTarget()
    {
        _attackAnimation = true;
        yield return new WaitForSeconds(0.6f);
        _audioAttack.Play();
        StartCoroutine(_enemy.GetComponent<PiecePieces>().GetHurt());
        yield return new WaitForSeconds(0.5f);
        _attackAnimation = false;
        yield return new WaitForSeconds(0.1f);
        _attackAnimation = true;
        yield return new WaitForSeconds(0.6f);
        _audioAttack.Stop();
        _audioAttack.Play();
        StartCoroutine(_enemy.GetComponent<PiecePieces>().Die());
        _enemy = null;
        _attackAnimation = false;
        yield return new WaitForSeconds(1f);
        _attacking = false;
        Move(attackDestination);
    }

    private IEnumerator Die()
    {
        _anim.SetBool("Dead",true);
        _audioGetHit.Play();
        yield return new WaitForSeconds(1f);
        _audioGetHit.Stop();
        _pieceManager.explosion.transform.position = gameObject.transform.position;
        _pieceManager.explosion.GetComponent<AudioSource>().Play();
        ExploderSingleton.Instance.ExplodeCracked(gameObject);
        gameObject.SetActive(false);
    }

    private IEnumerator GetHurt()
    {
        _anim.SetTrigger("hitted");
        _audioGetHit.Play();
        yield return new WaitForSeconds(.1f);
    }

    private void Rotate()
    {
        StartCoroutine(IsWhite ? RotateSmooth(-transform.eulerAngles.y) : RotateSmooth(180 - transform.eulerAngles.y));
    }

    private IEnumerator RotateSmooth(float value)
    {
        if (value > 180) value -= 360;
        if (value < -180) value += 360;
        const float x = 0.05f;
        for (var i = 0; i < 20;i++) {
            transform.Rotate(0, Mathf.Lerp(0, value, x), 0);
            yield return new WaitForSeconds(0.01f);
        }
    }
    
    private void TurnFinal()
    {
        _pieceManager.FinishedAnim();
        _arrived = true;
    }

    private void Update()
    {
        _anim.SetBool("moving",_moving);
        _anim.SetBool("attacking",_attackAnimation);

        if (_moving && !_navMeshAgent.pathPending)
        {
            if (_attacking && _navMeshAgent.remainingDistance < 5)
            {
                StartCoroutine(AttackTarget());
                _moving = false;
                _navMeshAgent.SetDestination(transform.position);
                _audioWalk.Stop();
            }
            else if(_navMeshAgent.remainingDistance < 0.25)
            {
                _moving = false;
                _audioWalk.Stop();
                _navMeshAgent.SetDestination(transform.position);
                Rotate();
                if(!_rock) TurnFinal();
            }
        }
    }
}
