cmake_minimum_required(VERSION 3.7)
set(CMAKE_SYSTEM_NAME Generic)
set(CMAKE_SYSTEM_VERSION 1)

# GCC ARM toolchain definition (arm-none-eabi).
# Ensure toolchain is in the PATH.
set(CMAKE_C_COMPILER_WORKS 1)
set(CMAKE_C_COMPILER arm-none-eabi-gcc)
set(CMAKE_CXX_COMPILER_WORKS 1)
set(CMAKE_CXX_COMPILER arm-none-eabi-g++)
set(CMAKE_ASM_COMPILER arm-none-eabi-gcc)
set(CMAKE_AR arm-none-eabi-ar)
set(CMAKE_OBJCOPY arm-none-eabi-objcopy)
set(CMAKE_OBJDUMP arm-none-eabi-objdump)
set(SIZE arm-none-eabi-size)

# Compilation and linking options.
set(COMMON_FLAGS "-mcpu={$Core$} -mfloat-abi={$FPType$} {$FPUnitOption$} -mthumb -mthumb-interwork -ffunction-sections -fdata-sections -g -fno-common -nostartfiles -fmessage-length=0 -specs=nosys.specs -specs=nano.specs")
set(CMAKE_CXX_FLAGS_INIT "${COMMON_FLAGS} -std=c++11 -fno-rtti -fno-exceptions")
set(CMAKE_C_FLAGS_INIT "${COMMON_FLAGS} -std=gnu99")
set(CMAKE_EXE_LINKER_FLAGS_INIT "-Wl,-gc-sections,--print-memory-usage -Tlinker.ld")
project({$ProjectName$} C CXX ASM)
set(CMAKE_CXX_STANDARD 11)
add_definitions({#-D{@Definitions@} #})
include_directories(. project)
link_directories(.)
add_executable(${PROJECT_NAME}.elf {#{@ProjectFiles@} #})
target_link_libraries(${PROJECT_NAME}.elf {#:{@Libraries@} #})

# HEX and BIN files generation.
add_custom_command(TARGET ${PROJECT_NAME}.elf POST_BUILD
  COMMAND ${CMAKE_OBJCOPY} -Oihex $<TARGET_FILE:${PROJECT_NAME}.elf> ${PROJECT_BINARY_DIR}/${PROJECT_NAME}.hex
  COMMAND ${CMAKE_OBJCOPY} -Obinary $<TARGET_FILE:${PROJECT_NAME}.elf> ${PROJECT_BINARY_DIR}/${PROJECT_NAME}.bin
  COMMENT "Created HEX and BIN files")
