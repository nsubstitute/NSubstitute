const copyToClipboardText = 'copy to clipboard';
const successCopyText = 'copied successfully';
const tooltipHiddenClassName = 'tooltip-hidden';
const tooltipClassName = 'tooltip';
const copyDisabledClassName = 'copy-disabled';

window.onload = () => {
    const headers = document.querySelectorAll('h1, h2, h3, h4');
    headersfiltered = [...headers].filter((x) => !x.classList.contains(copyDisabledClassName));
    headersfiltered.forEach(header => {
        const anchor = createHeadingAnchor(header);
        const tooltip = createTooltip(header);
        header.innerText = null;
        header.appendChild(anchor);
        header.appendChild(tooltip);
        addHoverEventHandlers(header, tooltip);
    });
}

const createHeadingAnchor = (header) => {
  const anchor = document.createElement('a');
  anchor.href = '#' + header.id;
  anchor.innerText = header.innerText;
  return anchor;
}

const createTooltip = (header) => {
  const tooltip = document.createElement('a');
  tooltip.classList.add(tooltipHiddenClassName)
  tooltip.classList.add(tooltipClassName)
  tooltip.innerHTML = copyToClipboardText;
  tooltip.addEventListener('click', () => {
    copyUrl(header, tooltip);
  });
  return tooltip;
}

const copyUrl = (header, tooltip) => {
  navigator.clipboard.writeText(header.firstChild.href);
  showCopyText(tooltip)
}

const addHoverEventHandlers = (element, tooltip) => {
  element.addEventListener('mouseover', () => {
    displayTooltipText(tooltip);
  });

  element.addEventListener('mouseleave', () => {
    hideTooltipText(tooltip);
    resetTooltipText(tooltip);
  });
}

const displayTooltipText = (tooltip) => {
  tooltip.classList.remove(tooltipHiddenClassName);
}

const hideTooltipText = (tooltip) => {
  tooltip.classList.add(tooltipHiddenClassName);
}

const showCopyText = (tooltip) => {
  tooltip.innerHTML = successCopyText;
  setTimeout(() => {
    resetTooltipText(tooltip);
    }, 1000);
}

const resetTooltipText = (tooltip) => {
  tooltip.innerHTML = copyToClipboardText;
} 