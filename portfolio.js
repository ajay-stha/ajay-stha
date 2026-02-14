AOS.init();

const cursor = document.getElementById('cursor-circle');
const stroke = document.getElementById('cursor-stroke');
let mouseX = 0;
let mouseY = 0;
let strokeX = 0;
let strokeY = 0;

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

const enlargeSelectors = [
  'a',
  '.interactive-card',
  '.group'
];

function setStrokeEnlarged(enlarged) {
  if (enlarged) {
    stroke.classList.add('enlarged');
  } else {
    stroke.classList.remove('enlarged');
  }
}

enlargeSelectors.forEach((selector) => {
  document.querySelectorAll(selector).forEach((el) => {
    el.addEventListener('mouseenter', () => setStrokeEnlarged(true));
    el.addEventListener('mouseleave', () => setStrokeEnlarged(false));
  });
});
