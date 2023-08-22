window.onload = () => {
    const headers = document.querySelectorAll('h2');
    headers.forEach(header => {
        header.classList.add('tooltip');
        const tooltipText = createTooltipTextElement();
        header.appendChild(tooltipText);
        header.addEventListener('click', () => copyUrl(header, tooltipText), true);
    });
}

const copyUrl = (header, tooltip) => {
    navigator.clipboard.writeText(header.firstChild.href);
    tooltip.classList.remove('tooltiptext-hidden');
    setTimeout(() => {
      tooltip.classList.add('tooltiptext-hidden');
      }, 500);
}

const createTooltipTextElement = () => {
  const tooltip = document.createElement('span');
  tooltip.classList.add('tooltiptext-hidden')
  tooltip.classList.add('tooltiptext')
  tooltip.innerHTML = 'copied to clipboard';
  return tooltip;
}