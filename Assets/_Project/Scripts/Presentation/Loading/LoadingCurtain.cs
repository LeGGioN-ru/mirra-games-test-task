using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace ClockApp.Presentation.Loading
{
    public sealed class LoadingCurtain : MonoBehaviour, ILoadingScreen
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _statusLabel;
        [SerializeField] private RectTransform _progressFill;
        [SerializeField] private float _fadeDuration = 0.4f;

        public void Show()
        {
            _canvasGroup.DOKill();
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
            gameObject.SetActive(true);
            SetProgress(0f);
        }

        public void SetStage(LoadingStage stage)
        {
            _statusLabel.text = stage switch
            {
                LoadingStage.SynchronizingTime => "Синхронизация времени…",
                LoadingStage.LoadingScene => "Загрузка часов…",
                _ => "Не удалось загрузить приложение"
            };
        }

        public void SetProgress(float progress)
        {
            _progressFill.anchorMax = new Vector2(Mathf.Clamp01(progress), 1f);
        }

        public async UniTask HideAsync(CancellationToken cancellationToken)
        {
            _canvasGroup.blocksRaycasts = false;

            await _canvasGroup
                .DOFade(0f, _fadeDuration)
                .SetUpdate(true)
                .SetLink(gameObject)
                .WithCancellation(cancellationToken);

            gameObject.SetActive(false);
        }
    }
}
