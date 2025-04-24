
document.addEventListener('DOMContentLoaded', function () {
    setTimeout(function () {
        document.body.classList.add('fade-in-body');
    }, 10);
});

    document.addEventListener('DOMContentLoaded', function() {
  const dyn = document.getElementById('main-hero');
  setTimeout(() => dyn.classList.add('fade-in'), 10);

  // observera förändringar i barn-noder
  new MutationObserver(() => {
        dyn.classList.remove('fade-in');
    setTimeout(() => dyn.classList.add('fade-in'), 10);
  }).observe(dyn, {childList: true });
});
