# 🎮 Unity+React 웹 기반 3D 리듬게임 (Step Up)

## 🎀 프로젝트 소개

🏷 **프로젝트 명 : Step Up - Cross-Platform Rhythm Game**

🗓️ **개발 기간 : 1/2(목) ~ 1/29(수)**

👥 **TEAM 핑꾸공쥬 : 최서희(팀장), 곽승훈, 박소희, 최윤아**

🧑‍🏫 **멘토 : 김승기**

---

### 🥰 서비스 구경 바로가기

🖥 **서비스 주소 : https://rhythm-game-website.vercel.app**

---

### ✅ 기획 배경

> PC와 모바일, 어디서든 즐길 수 있는 리듬 게임을 만들 수 있을까?

Unity 게임과 React 웹을 연동해, 사용자가 원하는 환경(PC/모바일)에서 게임을 즐길 수 있도록 크로스플랫폼 리듬 게임을 기획했다.

### ✅ 서비스 소개

> **Unity(PC), React(Web), 모바일 컨트롤러가 실시간으로 연동되는 크로스플랫폼 하이브리드 리듬 게임**

학교를 배경으로 한 3D 리듬 액션을, PC와 모바일을 오가며 즐길 수 있다.

- 리듬 게임 플레이 및 판정 시스템 (Perfect/Great/Good/Miss)
- 실시간 랭킹 및 점수 제출 (Supabase 연동)
- 인게임 결제 시스템 (Toss Payments 연동, 코인/아이템 구매)
- QR 코드로 스마트폰을 연결하는 모바일 컨트롤러

### 👥 서비스 대상

- 크로스플랫폼(PC↔모바일) 게임 경험에 관심 있는 사람들
- 새로운 방식의 모션/터치 조작 리듬 게임을 즐기고 싶은 사람들

---

## 💌 게임 화면 및 기능 소개

### ✅ 메인 페이지 및 게임 로비

- **직관적인 UI/UX** : 로비, 상점, 곡 선택 화면 등 직관적인 인터페이스 및 시각 효과
- **실시간 랭킹 시스템** : 곡별 점수 집계 및 서버 연동을 통한 TOP 랭킹 갱신
- **상점 및 인벤토리** : 아바타/아이템 구매 시스템과 인벤토리 관리

`(로비/상점 화면 스크린샷 추가 예정)`

### ✅ 곡 선택 및 플레이 모드

- **카드형 곡 선택 UI** : 앨범 아트를 강조한 슬라이드 방식
- **상세 곡 정보 제공** : 썸네일, 곡 제목, BPM, 난이도를 직관적으로 표시
- **듀얼 플레이 환경** : PC(마우스/키보드) / 모바일(터치·모션) 중 선택 가능

`(곡 선택 화면 스크린샷 추가 예정)`

### ✅ 인게임 플레이 및 결과 분석

- **3D 캐릭터 퍼포먼스** : 곡의 비트와 동기화된 캐릭터 댄스 모션
- **정밀 타격 시스템** : 버블 노트(Bubble Note) 인터페이스와 정확한 판정 로직
- **실시간 피드백** : 타격 이펙트 및 실시간 콤보 & 스코어 연출
- **상세 결과 리포트** : 랭크(Rank), 콤보, 판정별 세부 기록을 보여주는 통계형 결과 페이지

`(플레이/결과 화면 스크린샷 추가 예정)`

---

## 🕹️ 조작 방법

| 방식 | 설명 |
| --- | --- |
| 마우스 & 터치 | 판정선(Hit Zone)에 노트가 도달했을 때 해당 라인을 클릭/터치 |
| 모바일 컨트롤러 | QR 코드 스캔으로 연결 → **Shake**(가장 가까운 노트 자동 처리) 또는 **Touch**(4개 레인 직접 터치) |

### 판정 시스템

| 등급 | 판정 범위 | 점수 | 설명 |
| --- | --- | --- | --- |
| PERFECT | ±0.1s 이내 | +100점 | 완벽한 타이밍, 화려한 이펙트 발생 |
| GREAT | ±0.3s 이내 | +75점 | 준수한 타이밍 |
| GOOD | ±0.5s 이내 | +50점 | 조금 빗나갔지만 콤보 유지 |
| MISS | ±0.5s 초과 | 0점 | 콤보가 끊기고 체력 감소 |

HP는 100%에서 시작하며 MISS 시 10%씩 감소, 0이 되면 게임 오버 후 결과 화면으로 이동한다.

### 수록곡

| 곡 제목 | 장르 | BPM | 난이도 |
| --- | --- | --- | --- |
| GALAXIAS! | Electro Pop | 158 | ★★★☆☆ |
| Sodapop | Bubblegum Pop | 128 | ★★☆☆☆ |

---

## 💳 결제 & 🏆 랭킹 시스템

- **결제 흐름** : Unity 게임 SHOP → 웹 브라우저 자동 열림 → Toss Payments 결제 → Supabase DB 코인 반영
- **코인 패키지** : 100 / 500(+50 보너스) / 1,000(+150) / 5,000(+1,000)
- **랭킹 제출** : 게임 종료 시 로그인 사용자에 한해 점수·최대 콤보·판정별 기록이 Supabase에 자동 제출

---

## 🛠 기술 스택

### Frontend

![](https://img.shields.io/badge/React_18-61DAFB?style=flat-square&logo=react&logoColor=black)
![](https://img.shields.io/badge/TypeScript-3178C6?style=flat-square&logo=typescript&logoColor=white)
![](https://img.shields.io/badge/Vite-646CFF?style=flat-square&logo=vite&logoColor=white)
![](https://img.shields.io/badge/Tailwind_CSS-06B6D4?style=flat-square&logo=tailwindcss&logoColor=white)
![](https://img.shields.io/badge/Socket.IO-010101?style=flat-square&logo=socketdotio&logoColor=white)

### Backend & Database

![](https://img.shields.io/badge/Node.js-339933?style=flat-square&logo=nodedotjs&logoColor=white)
![](https://img.shields.io/badge/Express-000000?style=flat-square&logo=express&logoColor=white)
![](https://img.shields.io/badge/Supabase-3ECF8E?style=flat-square&logo=supabase&logoColor=white)

### Game Engine & Payment

![](https://img.shields.io/badge/Unity-000000?style=flat-square&logo=unity&logoColor=white)
![](https://img.shields.io/badge/Toss_Payments-0064FF?style=flat-square&logo=tosspayments&logoColor=white)

> React(Web)와 Unity(WebGL)를 Node.js/Express 브릿지 서버로 연결하고, 모바일 컨트롤러와는 Socket.IO(WebSocket)로 실시간 통신한다. 인증·DB·실시간 랭킹은 Supabase(BaaS) 하나로 통합해 별도 백엔드 없이 처리했다.

---

## 🚀 실행 방법

```bash
# 1. Unity 인증 브릿지 서버
node unity_auth_server.js
# → http://localhost:3001

# 2. React 웹
cd Frontend
npm install
npm run dev
# → http://localhost:5173

# 3. Unity 게임
# Unity Hub에서 프로젝트 열기 → Assets/Scenes/SchoolLobby 씬 실행 → Play
```

---

## 🗂 프로젝트 구조

```
└─📦 Rhythm_game
  ├─📂 Frontend                   # React 웹 애플리케이션
  │  └─📂 src
  │    ├─📂 pages/                # HomePage, LoginPage, RankingPage, PaymentPage, GamePage
  │    ├─📂 components/
  │    ├─📂 context/
  │    └─📂 services/
  ├─📂 Assets                     # Unity 게임 에셋
  │  ├─📂 Scenes/                 # SchoolLobby, Game_first(Galaxias), Game_second(Sodapop)
  │  └─📂 scripts/                # GameManager.cs, NetworkManager.cs, LoginManager.cs, SchoolLobbyManager.cs
  └─📜 unity_auth_server.js       # Unity ↔ React 인증 브릿지
```

---

## 📜 프로젝트 산출물

시연 영상은 훈련생 발표가 아닌, 세부 기능 소개와 화면 구동/기능 동작 여부를 보여주는 시연 형식으로 제작했다.

---

## 💬 자체 평가

**완성도 평가 : 7/10점 (개발 진행 중)**

- **달성 현황** : 리듬 게임 플레이 및 판정 시스템, 실시간 랭킹 및 점수 제출, 인게임 결제 시스템 구현
- **미달성 항목** : 목표였던 모바일 컨트롤러의 자이로 센서(Tilt) 조작, 외부 정식 배포 단계 마무리
- **잘한 부분** : 백엔드를 Supabase(BaaS) 하나로 통합해 불필요한 중계 서버를 없애고 아키텍처를 단순화
- **아쉬운 점** : 보안이나 디테일한 사용자 경험과 서비스 안정성 부분에서 시간과 경험이 부족해 타협한 점
- **추후 개선할 점** : 단순 터치를 넘어 자이로 센서(Tilt) 기능 완성으로 모바일 조작 가능성 강화, 서버 배포 및 최적화로 접근성 확보
- **느낀 점** : React 웹 프레임워크와 Unity WebGL 간 양방향 통신 브릿지 설계 및 통합 플랫폼 구현, 명확한 API 설계와 역할 분담을 통한 서로 다른 기술 스택 통합, 기획부터 DB 설계, 최종 클라이언트 빌드까지 서비스 개발 전 과정 주도적 수행

---

## 💙 팀원 소개

| 최서희 (팀장) | 곽승훈 | 박소희 | 최윤아 |
| --- | --- | --- | --- |
| UI/UX 디자인 시스템 구축 | Unity 게임 개발 | UI/UX 디자인 시스템 구축 | 게임 로직 구현 |
| 사용자 인터페이스 설계 | 데이터베이스 설계 및 연동 | 사용자 인터페이스 설계 | 웹 애플리케이션 개발 및 서버 구축 |
