# 카피바라 키우기 (Capybara Game)

Unity로 개발한 성장형 캐주얼 게임입니다.  
플레이어는 카피바라를 조작해 물고기를 먹으며 성장하고,  
점수 기반 난이도와 피버 타임 시스템을 통해 플레이 흐름이 변화합니다.

---

## 개발 환경
- Engine: Unity 6000.3.2f1
- Language: C#
- Platform: PC / WebGL / Android
- Genre: 캐주얼 · 성장형 액션
- Development: 개인 프로젝트

---

## 담당 역할
- 게임 전체 클라이언트 구조 설계 및 구현
- 플레이어 이동, 충돌, 성장 로직 구현
- 점수 기반 물고기 스폰 및 난이도 조절 시스템 구현
- 피버 타임, 산소 시스템 등 핵심 게임 플레이 로직 구현
- 월드 좌표 기반 UI(점수 라벨, 팝업) 시스템 구현
- 사운드 매니저 및 백그라운드 음소거 정책 구현
- 모바일/WebGL 환경 입력 대응

---

## 핵심 기능

### 플레이어 시스템
- 가상 조이스틱 기반 이동
- Rigidbody 기반 물리 이동
- 이동 방향에 따른 회전 보간 처리
- 화면 경계 Clamp 처리

관련 스크립트
PlayerController  
SimpleJoystick  
CameraFollow  

---

### 성장 및 점수 시스템
- 물고기 점수 ≤ 플레이어 점수일 경우 섭취 가능
- 섭취 시 점수 증가 및 플레이어 크기 증가
- 점수에 비례한 게임 난이도 상승

관련 스크립트  
PlayerCollision  
FishManager  
WorldSpaceScoreText  

---

### 피버 타임 시스템
- 일정 수의 물고기 섭취 시 피버 타임 진입
- 이동 속도 및 크기 증가
- 피버 전용 BGM 재생
- 종료 시 정상 상태 복귀

관련 스크립트  
PlayerFeverEffect  
PlayerCollision  

---

### 산소 시스템
- 시간이 지날수록 산소 감소
- 산소 부족 시 경고 UI 표시
- 산소 존 진입 시 회복
- 산소 고갈 시 게임 오버

관련 스크립트 
OxygenZone  
PlayerCollision  
PlayerUI  

---

### 물고기 스폰 & 난이도 조절
- 플레이어 점수 기반 확률적 스폰
- 고점수 물고기 개체 수 제한
- 점수에 비례한 이동 속도 증가
- 화면 밖 이탈 시 자동 제거

관련 스크립트 
FishManager  
FishMovement  

---

### UI & 연출
- 월드 좌표 기반 점수 라벨 표시
- 오브젝트 크기에 따른 UI 위치 자동 보정
- 점수 획득 및 사망 팝업 연출

관련 스크립트  
WorldSpaceScoreText  
WorldScorePopup  
WorldBonePopup  

---

### 사운드 시스템
- BGM / 효과음 분리 관리
- 일반 BGM / 피버 BGM 전환
- OS 백그라운드 및 포커스 해제 시 즉시 음소거
- WebGL(Apps in Toss) 가시성 정책 대응

관련 스크립트  
SoundManager  
AppAudioVisibilityController  

---

## 프로젝트 특징
- 점수 중심 구조로 자연스러운 난이도 상승
- 월드 오브젝트와 UI를 분리한 구조 설계
- 모바일 및 WebGL 환경을 고려한 입력·사운드 처리
