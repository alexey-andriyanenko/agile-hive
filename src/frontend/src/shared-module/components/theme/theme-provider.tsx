"use client";

import { ChakraProvider, createSystem, defaultConfig, defineConfig } from "@chakra-ui/react";
import {
  ColorModeProvider,
  type ColorModeProviderProps,
} from "src/shared-module/components/theme/color-mode.tsx";

const themeConfig = defineConfig({});
const theme = createSystem(defaultConfig, themeConfig);

export function ThemeProvider(props: ColorModeProviderProps) {
  return (
    <ChakraProvider value={theme}>
      <ColorModeProvider {...props} />
    </ChakraProvider>
  );
}
