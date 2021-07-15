using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{

    [SerializeField] bool isPlayerUnit;
    [SerializeField] BattleHud hud;
    

    public bool IsPlayerUnit
    {
        get
        {
            return isPlayerUnit;
        }
    }

    public BattleHud Hud
    {
        get
        {
            return hud;
        }
    }

    public Piece Piece { get; set; }

    Image image;
    Vector3 orignalPos;
    Color originalColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        orignalPos = image.transform.localPosition;
        originalColor = image.color;
    }

    // sets up what pices are being used
    public void SetUp(Piece piece)
    {
        Piece = piece;
        if (isPlayerUnit)
            image.sprite = Piece.Base.BackSpriten;
        else
            image.sprite = Piece.Base.FrontSprite;

        hud.gameObject.SetActive(true);
        hud.SetData(piece);

        PlayEnterAnimation();

        //eps 12 timestamp 12:50
        image.color = originalColor;
    }

    //dotween is a add on from the asset store ep 11
    
    public void PlayEnterAnimation()
    {
        if (isPlayerUnit)
            image.transform.localPosition = new Vector3(-500f, orignalPos.y);
        else
            image.transform.localPosition = new Vector3(500f, orignalPos.y);

        image.transform.DOLocalMoveX(orignalPos.x, 1f);
    }

    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
            sequence.Append(image.transform.DOLocalMoveX(orignalPos.x + 50f, 0.25f));
        else
            sequence.Append(image.transform.DOLocalMoveX(orignalPos.x - 50f, 0.25f));

        sequence.Append(image.transform.DOLocalMoveX(orignalPos.x, 0.2f));
    }

    public void PlayHitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.gray, 0.1f));
        sequence.Append(image.DOColor(originalColor, 0.1f));
    }

    public void PlayDeathAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(orignalPos.y - 150, 0.5f));
        sequence.Join(image.DOFade(0f, 0.5f));
    }

    public void Clear()
    {
        hud.gameObject.SetActive(false);
    }

}
