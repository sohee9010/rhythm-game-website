import React from 'react'
import { Link } from 'react-router-dom'

const HomePage: React.FC = () => {
  // Parallax ref removed to fix scroll issues

  return (
    <div className="min-h-screen bg-black text-white overflow-hidden font-neon selection:bg-purple-500 selection:text-white">

      {/* Hero Section */}
      <section className="relative h-screen flex items-center justify-center overflow-hidden">
        {/* Background Particles/Gradients */}
        <div className="absolute inset-0 z-0 pointer-events-none">
          <div className="absolute top-[-20%] left-[-10%] w-[500px] h-[500px] bg-purple-600/30 rounded-full blur-[100px] animate-pulse-glow"></div>
          <div className="absolute bottom-[-20%] right-[-10%] w-[600px] h-[600px] bg-blue-600/20 rounded-full blur-[120px] animate-pulse-glow" style={{ animationDelay: '1s' }}></div>
          <div className="absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 w-full h-full bg-[radial-gradient(ellipse_at_center,_var(--tw-gradient-stops))] from-transparent via-black/40 to-black"></div>
        </div>

        <div className="relative z-10 text-center px-4 flex flex-col items-center">

          {/* Logo removed as requested */}

          <p className="text-2xl md:text-3xl text-gray-300 mb-12 max-w-2xl mx-auto font-light tracking-wider">
            비트를 느껴보세요 <br />
            <span className="text-purple-400 font-bold mt-2 block neon-text text-4xl md:text-5xl">
              PLAY THE RHYTHM
            </span>
          </p>

          <div className="flex flex-col md:flex-row gap-8 justify-center items-center">

            <Link
              to="/game"
              className="group relative inline-block focus:outline-none"
            >
              <div className="absolute inset-0 bg-gradient-to-r from-purple-600 to-blue-600 rounded-full blur opacity-75 group-hover:opacity-100 transition duration-1000 group-hover:duration-200 animate-pulse-glow"></div>
              <div className="relative px-12 py-5 bg-black ring-1 ring-gray-600/50 rounded-full leading-none flex items-center">
                <span className="space-x-2 text-gray-100 group-hover:text-purple-300 transition-colors duration-200">
                  <span className="text-3xl font-bold tracking-widest">게임 시작</span>
                </span>
                <svg className="w-8 h-8 ml-4 text-purple-400 animate-pulse" fill="currentColor" viewBox="0 0 20 20">
                  <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM9.555 7.168A1 1 0 008 8v4a1 1 0 001.555.832l3-2a1 1 0 000-1.664l-3-2z" clipRule="evenodd" />
                </svg>
              </div>
            </Link>

          </div>
        </div>

        {/* Scroll Indicator */}
        <div className="absolute bottom-10 left-1/2 transform -translate-x-1/2 animate-bounce flex flex-col items-center opacity-70">
          <span className="mb-2 text-sm uppercase tracking-widest text-purple-400">Scroll</span>
          <svg className="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 14l-7 7m0 0l-7-7m7 7V3" />
          </svg>
        </div>
      </section>

      {/* Features Section */}
      <section id="features" className="py-32 relative z-10 bg-black/80 backdrop-blur-sm">
        <div className="max-w-7xl mx-auto px-6">
          <div className="text-center mb-24">
            <h2 className="text-4xl md:text-5xl font-bold mb-4 text-white neon-text">게임 특징</h2>
            <div className="w-24 h-1 bg-gradient-to-r from-transparent via-purple-500 to-transparent mx-auto"></div>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-8">

            {/* Feature 1 */}
            <div className="neon-border p-8 bg-white/5 hover:bg-white/10 transition-all duration-300 group">
              <div className="text-5xl mb-6 group-hover:scale-110 transition-transform duration-300">📱</div>
              <h3 className="text-2xl font-bold mb-4 text-purple-300">별도 장비 불필요</h3>
              <p className="text-lg text-gray-400 font-light leading-relaxed">
                <span className="text-cyan-400 font-semibold">스마트폰</span> 하나면 충분합니다. 고가의 센서나 장비 없이 당신의 휴대폰이 컨트롤러가 됩니다.
              </p>
            </div>

            {/* Feature 2 */}
            <div className="neon-border p-8 bg-white/5 hover:bg-white/10 transition-all duration-300 group">
              <div className="text-5xl mb-6 group-hover:scale-110 transition-transform duration-300">🏃</div>
              <h3 className="text-2xl font-bold mb-4 text-pink-300">실시간 모션 인식</h3>
              <p className="text-lg text-gray-400 font-light leading-relaxed">
                지연 없는 <span className="text-pink-400 font-semibold">실시간 동기화</span>를 경험하세요. 당신의 움직임이 즉시 화면에 반영됩니다.
              </p>
            </div>

            {/* Feature 3 */}
            <div className="neon-border p-8 bg-white/5 hover:bg-white/10 transition-all duration-300 group">
              <div className="text-5xl mb-6 group-hover:scale-110 transition-transform duration-300">🏆</div>
              <h3 className="text-2xl font-bold mb-4 text-cyan-300">리듬 액션</h3>
              <p className="text-lg text-gray-400 font-light leading-relaxed">
                노트를 치고, 멋진 포즈를 취하세요. <span className="text-cyan-400 font-semibold">글로벌 랭킹</span>에서 최고 점수에 도전하세요.
              </p>
            </div>

          </div>
        </div>
      </section>

      {/* CTA Section */}
      <section className="py-32 relative overflow-hidden">
        <div className="absolute inset-0 bg-gradient-to-t from-purple-900/40 to-black z-0"></div>
        <div className="max-w-4xl mx-auto px-6 text-center relative z-10">
          <h2 className="text-5xl md:text-7xl font-bold mb-8 text-white neon-text">춤출 준비 되셨나요?</h2>
          <p className="text-2xl text-gray-300 mb-12 font-light">
            무대는 준비되었습니다. 음악이 당신을 기다립니다.
          </p>
          <Link
            to="/game"
            className="inline-block px-12 py-5 bg-white text-black text-2xl font-bold rounded-full hover:bg-purple-500 hover:text-white hover:scale-105 transition-all duration-300 shadow-[0_0_20px_rgba(255,255,255,0.4)] hover:shadow-[0_0_40px_rgba(168,85,247,0.6)]"
          >
            지금 플레이
          </Link>
        </div>
      </section>

      {/* Version Footer */}
      <footer className="py-6 bg-black/90 border-t border-purple-500/20">
        <div className="max-w-7xl mx-auto px-6 text-center">
          <p className="text-gray-500 text-sm">
            Version: <span className="text-purple-400 font-mono">v1.1.0</span>
            {' '} | {' '}
            Build: <span className="text-cyan-400 font-mono">6859ebf</span>
            {' '} | {' '}
            Updated: <span className="text-gray-400">2026-01-13</span>
          </p>
          <p className="text-gray-600 text-xs mt-2">
            🎵 Audio Separation Fix - Galaxias & Sodapop No Longer Mix
          </p>
        </div>
      </footer>
    </div>
  )
}

export default HomePage