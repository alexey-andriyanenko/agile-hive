import React from "react";

import { Stack, Field, Input, Button } from "@chakra-ui/react";
import { useForm } from "react-hook-form";
import { observer } from "mobx-react-lite";

import { useAuthStore } from "src/auth-module/store";

import type { SignInFormValues } from "./sign-in-form.types.ts";

export const SignInForm: React.FC = observer(() => {
  const authStore = useAuthStore();
  const { register, formState, setError, handleSubmit } = useForm<SignInFormValues>();

  const onSubmit = async (data: SignInFormValues) => {
    try {
      await authStore.signIn({
        email: data.email,
        password: data.password,
      });
    } catch (error) {
      setError("email", { type: "manual", message: "Invalid email or password" });
      setError("password", { type: "manual", message: "Invalid email or password" });

      console.error("Sign-in failed:", error);
      // Handle error appropriately, e.g., show a notification or message
    }
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <Stack width="100%" gap="4">
        <Field.Root invalid={!!formState.errors.email}>
          <Field.Label>Email</Field.Label>
          <Input
            {...register("email", {
              required: {
                value: true,
                message: "Email is required",
              },
              pattern: {
                value: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
                message: "Invalid email address",
              },
            })}
          />
          <Field.ErrorText>{formState.errors.email?.message}</Field.ErrorText>
        </Field.Root>

        <Field.Root invalid={!!formState.errors.password}>
          <Field.Label>Password</Field.Label>
          <Input
            type="password"
            {...register("password", {
              required: {
                value: true,
                message: "Password is required",
              },
            })}
          />
          <Field.ErrorText>{formState.errors.password?.message}</Field.ErrorText>
        </Field.Root>

        <Button loading={formState.isSubmitting} type="submit">
          Submit
        </Button>
      </Stack>
    </form>
  );
});
