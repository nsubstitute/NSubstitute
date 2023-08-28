window.onload = () => {
    const headers = document.querySelectorAll('h1, h2, h3, h4');
    headers.forEach(header => {
        const anchor = createHeadingAnchor(header);
        const tooltip = createTooltip();
        header.innerText = null;
        header.appendChild(anchor);
        header.appendChild(tooltip);
        header.addEventListener('click', () => {
          handleClickEvent(header, tooltip);
        }, true);
    });
}

const createHeadingAnchor = (header) => {
  const anchor = document.createElement('a');
  anchor.href = '#' + header.id;
  anchor.innerText = header.innerText;
  return anchor;
}

const createTooltip = () => {
  const tooltip = document.createElement('span');
  tooltip.classList.add('tooltip-hidden')
  tooltip.classList.add('tooltip')
  tooltip.innerHTML = 'copied to clipboard';
  return tooltip;
}

const handleClickEvent = (header, tooltip) => {
  copyUrl(header);
  displayTooltipText(tooltip);
}

const copyUrl = (header) => {
  navigator.clipboard.writeText(header.firstChild.href);
}

const displayTooltipText = (tooltip) => {
  tooltip.classList.remove('tooltip-hidden');
  setTimeout(() => {
    tooltip.classList.add('tooltip-hidden');
    }, 1000);
}