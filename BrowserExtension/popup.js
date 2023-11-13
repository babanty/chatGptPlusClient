document.addEventListener(
  "DOMContentLoaded",
  function () {
    var copyButton = document.getElementById("copyButton");
    copyButton.addEventListener(
      "click",
      function () {
        // Получаем активную вкладку
        chrome.tabs.query(
          { active: true, currentWindow: true },
          function (tabs) {
            // Отправляем сообщение в content script
            chrome.scripting.executeScript(
              {
                target: { tabId: tabs[0].id },
                function: copyMarkdown,
              },
              (injectionResults) => {
                for (const frameResult of injectionResults)
                  console.log("Frame Title: " + frameResult.result);
              }
            );
          }
        );
      },
      false
    );
  },
  false
);

// Эта функция будет выполняться в контексте веб-страницы
function copyMarkdown() {
  // Находим элемент с нужным классом для текста
  const markdownElement = document.querySelector(
    '[class^="react-scroll-to-bottom--css"]'
  );
  // Проверяем наличие кнопки
  const buttonElement = document.querySelector(
    ".btn.relative.btn-neutral.whitespace-nowrap.border-0.md\\:border"
  );
  const buttonText = buttonElement ? buttonElement.innerText.trim() : "";

  // Находим кнопку с классами отправить
  const sendButtonElement = document.querySelector("button.absolute.p-1");
  // Определяем статус кнопки
  let sendButtonStatus;
  if (sendButtonElement) {
    // Если кнопка существует, проверяем, есть ли у неё атрибут disabled
    sendButtonStatus = sendButtonElement.hasAttribute("disabled") ? 1 : 2;
  } else {
    // Если кнопка не найдена
    sendButtonStatus = 0;
  }
  const stopButtonElement = document.querySelector(
    "[aria-label='Stop generating']"
  );

  const currentAddress = window.location.href;
  // Создаем объект для JSON
  let jsonObj = {
    text: markdownElement ? markdownElement.innerText : "", // Используем текст из markdownElement, если он существует
    buttonExists: !!buttonElement, // Статус наличия кнопки
    buttonText: buttonText, // Текст кнопки
    sendButton: sendButtonStatus, // Статус наличия и состояния кнопки "sendButton"
    currentAddress: currentAddress,
    stopButtonElement: !!stopButtonElement,
  };

  // Преобразуем объект в строку JSON
  const jsonText = JSON.stringify(jsonObj);

  // Создаем временный элемент textarea для копирования
  const textarea = document.createElement("textarea");
  textarea.value = jsonText;
  document.body.appendChild(textarea);
  // Выделяем и копируем текст в формате JSON
  textarea.select();
  document.execCommand("copy");
  // Удаляем временный элемент
  document.body.removeChild(textarea);

  console.log("Текст и информация о кнопке в формате JSON скопированы");
}
