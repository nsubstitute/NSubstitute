window.onload = () => {
    const headers = document.querySelectorAll("h2");
    headers.forEach(header => {
        header.classList.add('tooltip');
        const tooltip = document.createElement("span");
        tooltip.classList.add('tooltiptext-hidden')
        tooltip.classList.add('tooltiptext')
        tooltip.innerHTML = 'copied';
        header.appendChild(tooltip);
        header.addEventListener("click", () => copyUrl(header, tooltip), true);
    });
}

const copyUrl = (header, tooltip) => {
    navigator.clipboard.writeText(header.firstChild.href);
    tooltip.classList.remove('tooltiptext-hidden');
    setTimeout(() => {
      tooltip.classList.add('tooltiptext-hidden');
      }, 500);
}