import React from "react";
import { Flex, Stack, Heading, Text, Link } from "@chakra-ui/react";
import { Link as RouterLink } from "react-router-dom";

import { AuthRoutes } from "src/auth-module";

import { SignInForm } from "./sign-in-form";

const SignIn: React.FC = () => {
  return (
    <Flex flex="1" direction="column" justify="start" align="center" padding="80px 40px 40px 40px">
      <Stack width="450px" gap="10">
        <Flex justify="center">
          <Heading fontSize="4xl">Sign in to your account</Heading>
        </Flex>

        <SignInForm />

        <Flex justify="center">
          <Text>
            Don't have an account?{" "}
            <Link
              as={RouterLink}
              // @ts-expect-error Chakra UI's Link component supports `as` prop for rendering as a React Router Link
              to={AuthRoutes.register}
            >
              Sign up
            </Link>
          </Text>
        </Flex>
      </Stack>
    </Flex>
  );
};

export default SignIn;
