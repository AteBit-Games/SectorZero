using System;
using System.Collections;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Runtime.ReporterSystem
{
    [Serializable]
    public struct CardCreateRequestData
    {
        [SerializeField] public string content;
        [SerializeField] public string severity;
    }


    [Serializable]
    internal struct CardCreateResponseData
    {
        [SerializeField] public bool ok;
        [SerializeField] public string cardId;
    }

    public class CodecksCardCreator : MonoBehaviour
    {
        public string codecksURL = "https://api.codecks.io/user-report/v1/create-report";
        public string suggestionToken;
        public string issueToken;
        
        public delegate void CardCreationResultDelegate(bool success, string result);

        private static UnityWebRequest HttpPost(string url, string bodyJsonString)
        {
            var request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            return request;
        }

        public enum CodecksSeverity
        {
            None,
            Low,
            High,
            Critical
        }
        
        public void CreateNewCard(string text, CodecksSeverity severity = CodecksSeverity.None, CardCreationResultDelegate resultDelegate = null)
        {
            StartCoroutine(CreateNewCardCoroutine(text, severity, resultDelegate));
        }

        private IEnumerator CreateNewCardCoroutine(string text, CodecksSeverity severity = CodecksSeverity.None, CardCreationResultDelegate resultDelegate = null)
        {
            string tokenToUse;
            switch (severity)
            {
                case CodecksSeverity.None:
                    tokenToUse = suggestionToken;
                    break;
                case CodecksSeverity.Low: case CodecksSeverity.High: case CodecksSeverity.Critical:
                    tokenToUse = issueToken;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(severity), severity, null);
            }
            
            if (string.IsNullOrEmpty(tokenToUse))
            {
                resultDelegate?.Invoke(false, "Invalid Token");
                yield break;
            }

            UnityWebRequest request;
            try
            {
                string url = codecksURL + "?token=" + tokenToUse;

                string severityStr = severity switch
                {
                    CodecksSeverity.Low => "low",
                    CodecksSeverity.High => "high",
                    CodecksSeverity.Critical => "critical",
                    _ => null
                };

                CardCreateRequestData cardData = new CardCreateRequestData
                {
                    content = text,
                    severity = severityStr,
                };

                string json = JsonConvert.SerializeObject(cardData).Replace(",\"severity\":null", "");
                request = HttpPost(url, json);
            }
            catch (Exception ex)
            {
                resultDelegate?.Invoke(false, $"exception sending initial request: {ex}");
                yield break;
            }

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                resultDelegate?.Invoke(false, $"request unsuccessful: {request.result}  {request.error}");
                yield break;
            }

            CardCreateResponseData response;
            string resultString = request.downloadHandler.text;

            try
            {
                response = JsonConvert.DeserializeObject<CardCreateResponseData>(resultString);
            }
            catch (Exception ex)
            {
                resultDelegate?.Invoke(false, $"exception deserializing response: {ex}");
                yield break;
            }

            if (!response.ok)
            {
                resultDelegate?.Invoke(true, $"Codecks OK = false {resultString}");
                yield break;
            }

            resultDelegate?.Invoke(true, response.cardId);
        }
    }
}
