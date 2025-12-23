import React, { useEffect, useRef } from 'react'
import { Link } from 'react-router-dom'

const HomePage: React.FC = () => {
  const heroRef = useRef<HTMLDivElement>(null)

  useEffect(() => {
    const handleScroll = () => {
      if (heroRef.current) {
        const scrolled = window.scrollY
        heroRef.current.style.transform = `translateY(${scrolled * 0.5}px)`
      }
    }
    window.addEventListener('scroll', handleScroll)
    return () => window.removeEventListener('scroll', handleScroll)
  }, [])

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

        <div className="relative z-10 text-center px-4 flex flex-col items-center" ref={heroRef}>

          {/* Logo removed as requested */}

          <p className="text-2xl md:text-3xl text-gray-300 mb-12 max-w-2xl mx-auto font-light tracking-wider">
            FEEL THE BEAT <br />
            <span className="text-purple-400 font-bold mt-2 block neon-text text-4xl md:text-5xl">
              MOVE YOUR BODY
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
                  <span className="text-3xl font-bold tracking-widest">START GAME</span>
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
            <h2 className="text-4xl md:text-5xl font-bold mb-4 text-white neon-text">SYSTEM FEATURES</h2>
            <div className="w-24 h-1 bg-gradient-to-r from-transparent via-purple-500 to-transparent mx-auto"></div>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-8">

            {/* Feature 1 */}
            <div className="neon-border p-8 bg-white/5 hover:bg-white/10 transition-all duration-300 group">
              <div className="text-5xl mb-6 group-hover:scale-110 transition-transform duration-300">📱</div>
              <h3 className="text-2xl font-bold mb-4 text-purple-300">Wearable Free</h3>
              <p className="text-lg text-gray-400 font-light leading-relaxed">
                Just use your <span className="text-cyan-400 font-semibold">Smartphone</span>. It acts as your motion controller sensor. No expensive gear required.
              </p>
            </div>

            {/* Feature 2 */}
            <div className="neon-border p-8 bg-white/5 hover:bg-white/10 transition-all duration-300 group">
              <div className="text-5xl mb-6 group-hover:scale-110 transition-transform duration-300">🏃</div>
              <h3 className="text-2xl font-bold mb-4 text-pink-300">Real-time Motion</h3>
              <p className="text-lg text-gray-400 font-light leading-relaxed">
                Experience <span className="text-pink-400 font-semibold">Zero Latency</span> synchronization. Your physical moves are instantly reflected on screen.
              </p>
            </div>

            {/* Feature 3 */}
            <div className="neon-border p-8 bg-white/5 hover:bg-white/10 transition-all duration-300 group">
              <div className="text-5xl mb-6 group-hover:scale-110 transition-transform duration-300">�</div>
              <h3 className="text-2xl font-bold mb-4 text-cyan-300">Rhythm Action</h3>
              <p className="text-lg text-gray-400 font-light leading-relaxed">
                Hit the notes, strike the poses. Compete for the <span className="text-cyan-400 font-semibold">High Score</span> on our global leaderboard.
              </p>
            </div>

          </div>
        </div>
      </section>

      {/* CTA Section */}
      <section className="py-32 relative overflow-hidden">
        <div className="absolute inset-0 bg-gradient-to-t from-purple-900/40 to-black z-0"></div>
        <div className="max-w-4xl mx-auto px-6 text-center relative z-10">
          <h2 className="text-5xl md:text-7xl font-bold mb-8 text-white neon-text">READY TO DANCE?</h2>
          <p className="text-2xl text-gray-300 mb-12 font-light">
            The stage is set. The music is waiting.
          </p>
          <Link
            to="/game"
            className="inline-block px-12 py-5 bg-white text-black text-2xl font-bold rounded-full hover:bg-purple-500 hover:text-white hover:scale-105 transition-all duration-300 shadow-[0_0_20px_rgba(255,255,255,0.4)] hover:shadow-[0_0_40px_rgba(168,85,247,0.6)]"
          >
            PLAY NOW
          </Link>
        </div>
      </section>
    </div>
  )
}

export default HomePage