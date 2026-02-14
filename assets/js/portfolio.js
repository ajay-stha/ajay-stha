AOS.init({ once: true, duration: 700 });

const cursor = document.getElementById('cursor-circle');
const stroke = document.getElementById('cursor-stroke');
const backToTop = document.getElementById('back-to-top');
const progress = document.getElementById('scroll-progress');
const yearEl = document.getElementById('current-year');

if (yearEl) {
  yearEl.textContent = new Date().getFullYear();
}

let mouseX = 0;
let mouseY = 0;
let strokeX = 0;
let strokeY = 0;

if (cursor && stroke) {
  document.addEventListener('mousemove', (e) => {
    mouseX = e.clientX;
    mouseY = e.clientY;
    cursor.style.top = `${mouseY}px`;
    cursor.style.left = `${mouseX}px`;
  });

  function animateStroke() {
    strokeX += (mouseX - strokeX) * 0.18;
    strokeY += (mouseY - strokeY) * 0.18;
    stroke.style.top = `${strokeY}px`;
    stroke.style.left = `${strokeX}px`;
    requestAnimationFrame(animateStroke);
  }
  animateStroke();

  const enlargeSelectors = ['a', 'button', '.interactive-card', '.group'];

  function setStrokeEnlarged(enlarged) {
    stroke.classList.toggle('enlarged', enlarged);
  }

  enlargeSelectors.forEach((selector) => {
    document.querySelectorAll(selector).forEach((el) => {
      el.addEventListener('mouseenter', () => setStrokeEnlarged(true));
      el.addEventListener('mouseleave', () => setStrokeEnlarged(false));
    });
  });
}

function handleScrollUi() {
  const scrollTop = window.scrollY;
  const scrollHeight = document.documentElement.scrollHeight - window.innerHeight;
  const percent = scrollHeight > 0 ? (scrollTop / scrollHeight) * 100 : 0;

  if (progress) {
    progress.style.width = `${percent}%`;
  }

  if (backToTop) {
    backToTop.classList.toggle('show', scrollTop > 320);
  }
}

window.addEventListener('scroll', handleScrollUi);
handleScrollUi();

if (backToTop) {
  backToTop.addEventListener('click', () => {
    window.scrollTo({ top: 0, behavior: 'smooth' });
  });
}
