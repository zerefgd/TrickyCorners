using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    [SerializeField]
    private GameObject _explosionPrefab;

    [SerializeField]
    private Vector3[] _directions;

    [SerializeField]
    private float _startSpeed;

    private float speed;
    private int speedMagnitude;
    private int speedDirectionIndex;

    [SerializeField]
    private AudioClip _moveClip, _pointClip, _loseClip;

    [SerializeField]
    private AnimationClip _destoryClip;

    private void Awake()
    {
        speed = _startSpeed;
        speedMagnitude = 1;
        speedDirectionIndex = 0;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            speedDirectionIndex += speedMagnitude;
            if (speedDirectionIndex == -1) speedDirectionIndex = _directions.Length - 1;
            if (speedDirectionIndex == _directions.Length) speedDirectionIndex = 0;
            AudioManager.Instance.PlaySound(_moveClip);
        }
    }

    private void FixedUpdate()
    {
        Vector3 temp = transform.position;
        temp += speed * speedMagnitude * Time.fixedDeltaTime * _directions[speedDirectionIndex].normalized;
        transform.position = temp;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(Constants.Tags.OBSTACLE))
        {
            GameManager.Instance.EndGame();
            AudioManager.Instance.PlaySound(_loseClip);
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
            return;
        }
        if (collision.CompareTag(Constants.Tags.SCORE))
        {
            GameManager.Instance.UpdateScore();
            StartCoroutine(PlayCollisionAnimation(collision.gameObject));
            AudioManager.Instance.PlaySound(_pointClip);
            if (Random.Range(0f, 1f) > 0.6f) speedMagnitude *= -1;
            return;
        }
    }

    private IEnumerator PlayCollisionAnimation(GameObject target)
    {
        target.GetComponent<Collider2D>().enabled = false;
        Animator targetAnimator = target.GetComponent<Animator>();
        targetAnimator.Play(_destoryClip.name);        
        yield return new WaitForSeconds(_destoryClip.length);
        Destroy(target);
    }
}