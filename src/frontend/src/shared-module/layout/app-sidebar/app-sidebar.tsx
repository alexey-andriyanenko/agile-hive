import React from "react";
import { Box, Heading, Stack } from "@chakra-ui/react";
import { Link } from "react-router-dom";

import { useColorModeValue } from "src/shared-module/components/theme";

import type { AppSidebarProps, AppSidebarNavItemProps } from "./app-sidebar.types.ts";

export const AppSidebar: React.FC<AppSidebarProps> = ({ title, navItems }) => {
  return (
    <Box width="320px" minWidth="320px" padding="40px 10px 40px 40px" borderRightWidth="1px">
      <Stack>
        <Heading> {title} </Heading>

        <Stack gap="4">
          {navItems.map((item, i) => (
            <NavItem key={i} href={item.href} name={item.name} />
          ))}
        </Stack>
      </Stack>
    </Box>
  );
};

const NavItem: React.FC<AppSidebarNavItemProps> = ({ href, name }) => {
  return (
    <Box
      as={Link}
      // @ts-expect-error Chakra UI's Link component supports `as` prop for rendering as a React Router Link
      to={href}
      p={2}
      bgColor={useColorModeValue("gray.100", "gray.700")}
      borderRadius="md"
      _hover={{ bgColor: useColorModeValue("gray.200", "gray.600") }}
      _active={{ bgColor: useColorModeValue("gray.300", "gray.500") }}
    >
      {name}
    </Box>
  );
};
