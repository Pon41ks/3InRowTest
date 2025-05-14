using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _maxActionBarSlots = 7;
    [SerializeField] private float _flyDuration = 0.5f;
    [SerializeField] private Ease _flyEase = Ease.OutBack;

    [Header("References")]
    [SerializeField] private Transform _actionBarParent;
    [SerializeField] private Transform[] _actionBarSlots;
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private GameObject _losePanel;
    [SerializeField] private Button _reshuffleButton;

    private List<Figure> _actionBarFigures = new List<Figure>();
    private int _currentSlotIndex = 0;


    private void OnEnable()
    {
        EventManager.OnObjectSelected.AddListener(HandleFigureSelected);
        EventManager.OnGameWin.AddListener(Win);

    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    private void Win()
    {
        _winPanel.SetActive(true);
        _reshuffleButton.interactable = false;
    }

    private void OnDisable()
    {
        EventManager.OnObjectSelected.RemoveListener(HandleFigureSelected);
        EventManager.OnGameWin.RemoveListener(Win);
    }

    private void HandleFigureSelected(Figure selectedFigure)
    {
        if (_currentSlotIndex >= _maxActionBarSlots) return;

        Vector3 targetPosition = _actionBarSlots[_currentSlotIndex].position;
        targetPosition.z = 0;

        Vector3 startPosition = selectedFigure.transform.position;
        startPosition.z = 0;
        selectedFigure.transform.position = startPosition;

        selectedFigure.transform.DOMove(
            targetPosition,
            _flyDuration
        ).SetEase(_flyEase).OnComplete(() => {
            Vector3 finalPosition = selectedFigure.transform.position;
            finalPosition.z = 0;
            selectedFigure.transform.position = finalPosition;
            selectedFigure.transform.rotation = Quaternion.identity;
            _actionBarFigures.Add(selectedFigure);
            _currentSlotIndex++;
            CheckMatches();
            CheckGameConditions();
        });
    }

    private void CheckMatches()
    {
        var groups = _actionBarFigures
            .GroupBy(f => new { f.Shape, f.FrameColor, f.Animal })
            .Where(g => g.Count() >= 3);

        foreach (var group in groups)
        {
            var figuresToRemove = group.Take(3).ToList();

            foreach (var figure in figuresToRemove)
            {
                _actionBarFigures.Remove(figure);
                figure.transform.DOScale(0, 0.3f).OnComplete(() => Destroy(figure.gameObject));
            }

            UpdateActionBarPositions();
            _currentSlotIndex -= 3;
        }
    }

    private void UpdateActionBarPositions()
    {
        for (int i = 0; i < _actionBarFigures.Count; i++)
        {
            Vector3 targetPostion = _actionBarSlots[i].position;
            targetPostion.z = 0;
            _actionBarFigures[i].transform.DOMove(
                targetPostion,
                0.3f
            );
        }
    }

    private void CheckGameConditions()
    {
        if (_currentSlotIndex >= _maxActionBarSlots)
        {
           _losePanel.SetActive(true);
            _reshuffleButton.interactable = false;
            EventManager.SetGameLose();
            return;
        }
    }

}
