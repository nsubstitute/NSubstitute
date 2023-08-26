window.onload = () => {
    const headers = document.querySelectorAll('h2');
    headers.forEach(header => {
        const tooltipText = createTooltipTextElement();
        header.appendChild(tooltipText);
        header.addEventListener('click', () => {
          handleClickEvent(header, tooltipText);
        }, true);
    });
}

const createTooltipTextElement = () => {
  const tooltip = document.createElement('span');
  tooltip.classList.add('tooltiptext-hidden')
  tooltip.classList.add('tooltiptext')
  tooltip.innerHTML = 'copied to clipboard';
  return tooltip;
}

const handleClickEvent = (header, tooltipText) => {
  copyUrl(header);
  displayTooltipText(tooltipText);
}

const copyUrl = (header) => {
    navigator.clipboard.writeText(header.firstChild.href);
}

const displayTooltipText = (tooltip) => {
  tooltip.classList.remove('tooltiptext-hidden');
  setTimeout(() => {
    tooltip.classList.add('tooltiptext-hidden');
    }, 1000);
}