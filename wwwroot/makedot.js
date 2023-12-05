const canvas = document.getElementById('canvas');
const ctx = canvas.getContext('2d');
const dots = []; // 水玉を保持する配列
let animationFrameId = null; // アニメーションフレームID
const minSize = 40; // 水玉の最低サイズ
let useGradient = false; // グラデーションを使用するかどうかのフラグ

function resizeCanvas() {
    // canvas.width = window.innerWidth;
    // canvas.height = window.innerHeight;
    canvas.width = 1024;
    canvas.height = 1024;
}

// ウィンドウのリサイズ時にCanvasのサイズを調整
// window.addEventListener('resize', resizeCanvas);

// 初期サイズを設定
resizeCanvas();

// canvas.addEventListener('click', function() {
//     createDotWithGrowingAnimation();
// });

// グラデーションの使用を切り替えるための関数
function toggleGradient() {
    useGradient = !useGradient;
}

function createDotWithGrowingAnimation(volume) {

    console.log("make dot");

    const x = Math.random() * canvas.width;
    const y = Math.random() * canvas.height;
    // const maxSize = Math.random() * 80 + minSize; // 最終サイズはランダムで、最低サイズを加える
    // 音量からサイズを決定（音量の幅は0.01~0.5程度）”
    const maxSize = volume * 80 + minSize; // 最終サイズはランダムで、最低サイズを加える
    const color = `rgb(${Math.random() * 255}, ${Math.random() * 255}, ${Math.random() * 255})`; // 色はランダム
    const dot = { x, y, size: minSize, maxSize, color, growing: true }; // 初期サイズを最低サイズに設定
    dots.push(dot); // 水玉を配列に保存

    if (!animationFrameId) {
        animationFrameId = requestAnimationFrame(draw);
    }
}

function draw() {
    ctx.clearRect(0, 0, canvas.width, canvas.height); // Canvasをクリア
    dots.forEach(dot => {
        if (dot.growing) {
            dot.size += 2; // 水玉を広げる
            if (dot.size >= dot.maxSize) {
                dot.growing = false; // 最終サイズに達したら成長を止める
            }
        }

        // グラデーションの使用
        if (useGradient) {
            const gradient = ctx.createRadialGradient(dot.x, dot.y, 0, dot.x, dot.y, dot.size / 2);
            gradient.addColorStop(0, dot.color);
            gradient.addColorStop(1, 'rgba(0, 0, 0, 0)'); // 外側を透明に
            ctx.fillStyle = gradient;
        } else {
            ctx.fillStyle = dot.color;
        }

        ctx.beginPath();
        ctx.arc(dot.x, dot.y, dot.size / 2, 0, Math.PI * 2);
        ctx.fill();
    });
    animationFrameId = requestAnimationFrame(draw);
}

function exportCanvasAsImage() {
    const imageDataUrl = canvas.toDataURL('image/png');
    return imageDataUrl;
}
