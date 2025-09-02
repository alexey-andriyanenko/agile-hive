import React from "react";
import { useForm } from "react-hook-form";
import { Stack, Field, Input, Button } from "@chakra-ui/react";

import type { SignUpFormValues } from "./sign-up-form.types.ts";

export const SignUpForm: React.FC = () => {
  const { register, formState, handleSubmit } = useForm<SignUpFormValues>();

  const onSubmit = (data: SignUpFormValues) => {
    console.log("Form submitted with data:", data);
    // Handle form submission logic here
  };

  const validatePassword = (value: string) => {
    const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{9,}$/;
    if (!passwordRegex.test(value)) {
      return "Password must be at least 9 characters long, contain at least one uppercase letter, one lowercase letter, one number, and one special character.";
    }

    return true;
  };

  const validateConfirmPassword = (value: string, formValues: SignUpFormValues) => {
    if (value !== formValues.password) {
      return "Passwords do not match.";
    }

    return true;
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <Stack width="100%" gap="4">
        <Field.Root invalid={!!formState.errors.firstName}>
          <Field.Label>First Name</Field.Label>
          <Input
            {...register("firstName", {
              required: {
                value: true,
                message: "First name is required",
              },
            })}
          />
          <Field.ErrorText>{formState.errors.firstName?.message}</Field.ErrorText>
        </Field.Root>

        <Field.Root invalid={!!formState.errors.lastName}>
          <Field.Label>Last name</Field.Label>
          <Input
            {...register("lastName", {
              required: {
                value: true,
                message: "Last name is required",
              },
            })}
          />
          <Field.ErrorText>{formState.errors.lastName?.message}</Field.ErrorText>
        </Field.Root>

        <Field.Root invalid={!!formState.errors.email}>
          <Field.Label>Username</Field.Label>
          <Input
            {...register("username", {
              required: {
                value: true,
                message: "Username is required",
              },
            })}
          />
          <Field.ErrorText>{formState.errors.email?.message}</Field.ErrorText>
        </Field.Root>

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
              validate: validatePassword,
            })}
          />
          <Field.ErrorText>{formState.errors.password?.message}</Field.ErrorText>
        </Field.Root>

        <Field.Root invalid={!!formState.errors.confirmPassword}>
          <Field.Label>Confirm Password</Field.Label>
          <Input
            type="password"
            {...register("confirmPassword", {
              required: {
                value: true,
                message: "Confirm password is required",
              },
              validate: validateConfirmPassword,
            })}
          />
          <Field.ErrorText>{formState.errors.confirmPassword?.message}</Field.ErrorText>
        </Field.Root>

        <Field.Root invalid={!!formState.errors.organizationName}>
          <Field.Label>Organization name</Field.Label>
          <Input {...register("organizationName")} />
          <Field.ErrorText>{formState.errors.organizationName?.message}</Field.ErrorText>
        </Field.Root>

        <Button type="submit">Submit</Button>
      </Stack>
    </form>
  );
};
