Cursor
===

# 1. Cursor
커서에 대한 관리가 필요한 이유는 게임 일반적인 상황에서는 커서가 필요 없는데 UI 조작과 같은 상황에서는 커서가 필요하기 때문이다. 따라서 커서가 필요한 시기, 필요 없는 시기를 구분해야할 필요가 있다.

# 2. When?
# 2.1 Cursor Invisible
- Play Scene에 들어온 순간

# 2.2 Cursor Visible
- UI Scene 조작중
- alt를 누르는 동안

# 3. Where?
일단은 UI Manager에 넣자. 일단 Input을 처리하고 있고 UI Scene 조작 중인지 바로 알 수 있다.