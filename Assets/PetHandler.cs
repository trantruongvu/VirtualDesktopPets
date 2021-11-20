using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class PetHandler : MonoBehaviour, IDropHandler
{
    public RectTransform canvas;
    RectTransform currentObj;
    Animator petAnimator;
    private AnimatorClipInfo[] clipInfo;

    public Vector3 reverseScale = new Vector3(-1, 1, 1);
    float speed = 0.5f;

    // Movement status
    bool isMoving = false;
    (Vector3 position, float time) newMoveVector = (Vector3.zero, 0);

    // Start is called before the first frame update
    void Start()
    {
        currentObj = GetComponent<RectTransform>();
        petAnimator = GetComponent<Animator>();
        StartIdle();
    }

    public void StartIdle()
    {
        Debug.Log("StartIdle");
        int randomIdleTime = Random.Range(5, 10);
        StartCoroutine(waitForIdle(randomIdleTime));
        
        petAnimator.SetBool("isIdle", true);
        petAnimator.SetBool("isMove", false);
    }

    IEnumerator waitForIdle(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        StartMoving();
    }

    public void StartMoving()
    {
        newMoveVector = GetRandomPosition();
        Sequence moveSeq = DOTween.Sequence();
        moveSeq.Append(currentObj.DOMove(newMoveVector.position, newMoveVector.time));
        moveSeq.SetEase(Ease.Linear);
        moveSeq.OnComplete(() =>
        {
            StartIdle();
        });
        moveSeq.Play();

        petAnimator.SetBool("isIdle", false);
        petAnimator.SetBool("isMove", true);
    }

    private (Vector3, float) GetRandomPosition()
    {
        int randomUp = Random.Range(0, 2);
        float randomX;
        float randomDistance = Random.Range(250, 1000);

        if (randomUp == 0)
        {
            randomX = currentObj.position.x + randomDistance;
            transform.localScale = Vector3.one;
        }
            
        else
        {
            randomX = currentObj.position.x - randomDistance;
            transform.localScale = reverseScale;
        }

        float randomY = currentObj.position.y + Random.Range(-250, 250);

        float randomTime = randomDistance / 100;
        Debug.Log("randomTime: " + randomTime);

        if (randomX < 50 || randomX > canvas.rect.width - 50)
            return GetRandomPosition();

        if (randomY < 50 || randomY > canvas.rect.height - 50)
            return GetRandomPosition();

        return (new Vector3(randomX, randomY, 0), randomTime);
    }

    // Khi drop món đồ lên người
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");
        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            StartEating();
            eventData.pointerDrag.SetActive(false);
        }
    }

    public void StartEating()
    {
        StopAllCoroutines();
        petAnimator.SetBool("isIdle", false);
        petAnimator.SetBool("isMove", false);
        petAnimator.SetBool("isEat", true);
        StartCoroutine(waitForEat(2));
    }

    IEnumerator waitForEat(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        petAnimator.SetBool("isEat", false);
        StartIdle();
    }

    public string GetCurrentAnimtionName()
    {
        clipInfo = petAnimator.GetCurrentAnimatorClipInfo(0);
        return clipInfo[0].clip.name;
    }
}
